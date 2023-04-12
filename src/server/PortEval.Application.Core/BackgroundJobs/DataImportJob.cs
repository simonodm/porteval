using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PortEval.Application.Core.Common.BulkImportExport;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.BackgroundJobs;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Core.BackgroundJobs
{
    public class DataImportJob : IDataImportJob
    {
        private const string CSV_DELIMITER = ",";

        private readonly IServiceProvider _serviceProvider;
        private readonly IDataImportRepository _importRepository;
        private readonly INotificationService _notificationService;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly CsvConfiguration _csvConfig;
        private readonly List<RawRowErrorLogEntry> _parsingErrors;

        public DataImportJob(IServiceProvider serviceProvider, IDataImportRepository importRepository, IFileSystem fileSystem, ILoggerFactory loggerFactory, INotificationService notificationService)
        {
            _serviceProvider = serviceProvider;
            _importRepository = importRepository;
            _fileSystem = fileSystem;
            _notificationService = notificationService;
            _logger = loggerFactory.CreateLogger(typeof(DataImportJob));
            _parsingErrors = new List<RawRowErrorLogEntry>();
            _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = CSV_DELIMITER,
                ReadingExceptionOccurred = (args) =>
                {
                    var context = args.Exception.Context;
                    _parsingErrors.Add(new RawRowErrorLogEntry(context.Parser.Record, $"Failed to parse value \"{context.Parser[context.Reader.CurrentIndex]}\""));
                    return false;
                }
            };
        }

        public async Task RunAsync(Guid importId, string inputFileName, string logPath)
        {
            _logger.LogInformation($"Processing import {importId}.");

            var importEntry = await _importRepository.FindAsync(importId);
            if (importEntry == null)
            {
                return;
            }

            try
            {
                using var fs = _fileSystem.FileStream.Create(inputFileName, FileMode.Open);
                using var reader = new StreamReader(fs);
                using var csv = new CsvReader(reader, _csvConfig);
                csv.RegisterImportClassMaps();

                importEntry.ChangeStatus(ImportStatus.InProgress);
                importEntry.IncreaseVersion();
                _importRepository.Update(importEntry);
                await _importRepository.UnitOfWork.CommitAsync();

                await ProcessImportFromType(importEntry.TemplateType, csv, logPath);

                importEntry.ChangeStatus(ImportStatus.Finished);
                importEntry.AddErrorLog(logPath);
                await _notificationService.SendNotificationAsync(NotificationType.NewDataAvailable,
                    "Data import has been processed.");
            }
            catch (CsvHelperException ex)
            {
                _logger.LogError(ex.ToString());
                importEntry.ChangeStatus(ImportStatus.Error, $"Error: failed to process received data as {importEntry.TemplateType}.");
            }
            catch (PortEvalException ex)
            {
                _logger.LogError(ex.ToString());
                importEntry.ChangeStatus(ImportStatus.Error, $"Error: {ex.Message}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                importEntry.ChangeStatus(ImportStatus.Error, "Internal error.");
            }
            finally
            {
                importEntry.IncreaseVersion();
                _importRepository.Update(importEntry);
                await _importRepository.UnitOfWork.CommitAsync();
                DeleteFile(inputFileName);
            }
        }

        private async Task ProcessImportFromType(TemplateType templateType, CsvReader reader, string logPath)
        {
            switch (templateType)
            {
                case TemplateType.Portfolios:
                    await ProcessImport<PortfolioDto>(reader, logPath);
                    break;
                case TemplateType.Positions:
                    await ProcessImport<PositionDto>(reader, logPath);
                    break;
                case TemplateType.Instruments:
                    await ProcessImport<InstrumentDto>(reader, logPath);
                    break;
                case TemplateType.Prices:
                    await ProcessImport<InstrumentPriceDto>(reader, logPath);
                    break;
                case TemplateType.Transactions:
                    await ProcessImport<TransactionDto>(reader, logPath);
                    break;
                default:
                    break;
            }
        }

        private async Task ProcessImport<TRow>(CsvReader reader, string logPath)
        {
            var processor = _serviceProvider.GetRequiredService<IImportProcessor<TRow>>();
            var result = await processor.ImportRecordsAsync(reader.GetRecords<TRow>().ToList());
            SaveErrorLog(result.ErrorLog, _parsingErrors, logPath);
        }

        private void SaveErrorLog<T>(IEnumerable<ProcessedRowErrorLogEntry<T>> processedErrorLog, IEnumerable<RawRowErrorLogEntry> parsingErrorLog, string filename)
        {
            using var fs = _fileSystem.FileStream.Create(filename, FileMode.Create);
            using var sw = new StreamWriter(fs);
            using var csv = new CsvWriter(sw, _csvConfig);
            csv.RegisterImportClassMaps();

            csv.WriteErrorHeaders<T>();
            foreach (var entry in parsingErrorLog)
            {
                csv.WriteErrorEntry(entry);
            }
            foreach (var entry in processedErrorLog)
            {
                csv.WriteErrorEntry(entry);
            }
        }

        private void DeleteFile(string filePath)
        {
            if (_fileSystem.File.Exists(filePath))
            {
                _fileSystem.File.Delete(filePath);
            }
        }
    }
}
