using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Hangfire;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Services;

/// <inheritdoc cref="ICsvImportService" />
public class CsvImportService : ICsvImportService
{
    private readonly IFileSystem _fileSystem;
    private readonly IDataImportQueries _importDataQueries;
    private readonly IDataImportRepository _importRepository;

    private readonly IBackgroundJobClient _jobClient;
    private readonly string _storagePath;

    /// <summary>
    ///     Initializes the service.
    /// </summary>
    public CsvImportService(IDataImportRepository importRepository, IBackgroundJobClient jobClient,
        IFileSystem fileSystem, IDataImportQueries importDataQueries, string storagePath)
    {
        _importRepository = importRepository;
        _jobClient = jobClient;
        _fileSystem = fileSystem;
        _storagePath = storagePath;
        _importDataQueries = importDataQueries;
    }

    /// <inheritdoc />
    public async Task<OperationResponse<IEnumerable<CsvTemplateImportDto>>> GetAllImportsAsync()
    {
        var imports = await _importDataQueries.GetAllImportsAsync();

        return new OperationResponse<IEnumerable<CsvTemplateImportDto>>
        {
            Response = imports.Select(AssignErrorLogUrl)
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<CsvTemplateImportDto>> GetImportAsync(Guid id)
    {
        var import = await _importDataQueries.GetImportAsync(id);

        return new OperationResponse<CsvTemplateImportDto>
        {
            Status = import != null ? OperationStatus.Ok : OperationStatus.NotFound,
            Message = import != null ? "" : $"Import {id} does not exist.",
            Response = import != null ? AssignErrorLogUrl(import) : null
        };
    }

    /// <inheritdoc />
    public async Task<OperationResponse<CsvTemplateImportDto>> StartImportAsync(Stream inputFileStream,
        TemplateType templateType)
    {
        var guid = Guid.NewGuid();

        var tempFilePath = GetTemporaryFilePath(guid);
        var logFilePath = GetErrorLogPath(guid);

        await using var fs = _fileSystem.File.OpenWrite(tempFilePath);
        await inputFileStream.CopyToAsync(fs);
        fs.Close();

        var importEntry = await SaveNewImportEntry(guid, templateType);
        _jobClient.Enqueue<IDataImportJob>(job => job.RunAsync(guid, tempFilePath, logFilePath));

        return await GetImportAsync(importEntry.Id);
    }

    /// <inheritdoc />
    public OperationResponse<Stream> TryGetErrorLog(Guid guid)
    {
        try
        {
            var stream = _fileSystem.File.OpenRead(GetErrorLogPath(guid));
            return new OperationResponse<Stream>
            {
                Response = stream
            };
        }
        catch (Exception)
        {
            return new OperationResponse<Stream>
            {
                Status = OperationStatus.Error,
                Message = $"Failed to retrieve error log for ID {guid}"
            };
        }
    }

    /// <inheritdoc />
    public OperationResponse<Stream> GetCsvTemplate(TemplateType templateType)
    {
        var path = GetTemplatePath(templateType);
        if (string.IsNullOrWhiteSpace(path))
            return new OperationResponse<Stream>
            {
                Status = OperationStatus.Error,
                Message = "Failed to retrieve the template."
            };

        if (!_fileSystem.File.Exists(path)) GenerateTemplate(path, templateType);

        return new OperationResponse<Stream>
        {
            Response = _fileSystem.File.OpenRead(path)
        };
    }

    private string GetTemporaryFilePath(Guid guid)
    {
        return Path.Combine(_storagePath, guid + "_import.csv");
    }

    private string GetErrorLogPath(Guid guid)
    {
        return Path.Combine(_storagePath, guid + "_log.csv");
    }

    private string GetTemplatePath(TemplateType templateType)
    {
        return Path.Combine(_storagePath, templateType.ToString().ToLower() + "_template.csv");
    }

    private void GenerateTemplate(string path, TemplateType templateType)
    {
        using var fs = _fileSystem.File.OpenWrite(path);
        using var sw = new StreamWriter(fs);
        using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
        csv.RegisterImportClassMaps();

        var rowType = templateType.GetRowType();
        csv.WriteHeader(rowType);
        sw.Flush();
    }

    private async Task<DataImport> SaveNewImportEntry(Guid guid, TemplateType templateType)
    {
        var importEntry = DataImport.Create(guid, DateTime.UtcNow, templateType);
        _importRepository.Add(importEntry);
        await _importRepository.UnitOfWork.CommitAsync();

        return importEntry;
    }

    private CsvTemplateImportDto AssignErrorLogUrl(CsvTemplateImportDto importDto)
    {
        if (importDto.ErrorLogAvailable) importDto.ErrorLogUrl = $"/imports/{importDto.ImportId}/log";

        return importDto;
    }
}