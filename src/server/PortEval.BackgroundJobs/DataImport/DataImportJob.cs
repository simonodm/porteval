using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.BulkImportExport;
using PortEval.Application.Services.Extensions;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Enums;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace PortEval.BackgroundJobs.DataImport
{
    public class DataImportJob : IDataImportJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDataImportRepository _importRepository;
        private readonly ILogger _logger;
        private readonly CsvConfiguration _csvConfig;

        public DataImportJob(IServiceProvider serviceProvider, IDataImportRepository importRepository, ILoggerFactory loggerFactory)
        {
            _serviceProvider = serviceProvider;
            _importRepository = importRepository;
            _logger = loggerFactory.CreateLogger(typeof(DataImportJob));
            _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
            };
        }

        public async Task Run(Domain.Models.Entities.DataImport importEntry, string inputFileName, string logPath)
        {
            try
            {
                using var reader = new StreamReader(inputFileName);
                using var csv = new CsvReader(reader, _csvConfig);
                csv.RegisterImportClassMaps();

                await ProcessImportFromType(importEntry.TemplateType, csv, logPath);

                importEntry.ChangeStatus(ImportStatus.Finished);
                importEntry.AddErrorLog(logPath);
                _importRepository.Update(importEntry);
                await _importRepository.UnitOfWork.CommitAsync();
            }
            catch (CsvHelperException ex)
            {
                _logger.LogError(ex.Message);
                importEntry.ChangeStatus(ImportStatus.Error, $"Error: failed to process received data as {importEntry.TemplateType}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                importEntry.ChangeStatus(ImportStatus.Error, "Internal error.");
            }
            finally
            {
                _importRepository.Update(importEntry);
                await _importRepository.UnitOfWork.CommitAsync();
                DeleteFile(inputFileName);
            }
        }

        private async Task ProcessImportFromType(TemplateType templateType, CsvReader reader, string logPath)
        {
            switch(templateType)
            {
                case TemplateType.Portfolios:
                    await ProcessImport<PortfolioDto, PortfolioImportProcessor>(reader, logPath);
                    break;
                case TemplateType.Positions:
                    await ProcessImport<PositionDto, PositionImportProcessor>(reader, logPath);
                    break;
                case TemplateType.Instruments:
                    await ProcessImport<InstrumentDto, InstrumentImportProcessor>(reader, logPath);
                    break;
                case TemplateType.Prices:
                    await ProcessImport<InstrumentPriceDto, PriceImportProcessor>(reader, logPath);
                    break;
                case TemplateType.Transactions:
                    await ProcessImport<TransactionDto, TransactionImportProcessor>(reader, logPath);
                    break;
                default:
                    break;
            }
        }

        private async Task ProcessImport<TRow, TProcessor>(CsvReader reader, string logPath)
            where TProcessor : IImportProcessor<TRow>            
        {
            var processor = _serviceProvider.GetRequiredService<TProcessor>();
            var result = await processor.ProcessImport(reader.GetRecords<TRow>());
            SaveErrorLog(result, logPath);
        }

        private void SaveErrorLog<T>(ImportResult<T> importResult, string filename)
        {
            using var sw = new StreamWriter(filename);
            using var csv = new CsvWriter(sw, _csvConfig);
            csv.RegisterImportClassMaps();

            csv.WriteErrorHeaders<T>();
            foreach (var entry in importResult.ErrorLog)
            {
                csv.WriteErrorEntry(entry);
            }
        }

        private void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
