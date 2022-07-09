using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.BulkImportExport;
using PortEval.Application.Services.BulkImportExport.ClassMaps;
using PortEval.Application.Services.Interfaces.BackgroundJobs;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.Domain.Models.Enums;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

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

        public async Task Run(Guid importId, string inputFileName, TemplateType templateType, string logPath)
        {
            var importEntry = new Domain.Models.Entities.DataImport(importId, templateType, ImportStatus.InProgress);
            _importRepository.Add(importEntry);
            await _importRepository.UnitOfWork.CommitAsync();

            try
            {
                using var reader = new StreamReader(inputFileName);
                using var csv = new CsvReader(reader, _csvConfig);
                csv.Context.RegisterClassMap<PortfolioClassMap>();
                csv.Context.RegisterClassMap<PositionImportClassMap>();
                csv.Context.RegisterClassMap<TransactionClassMap>();
                csv.Context.RegisterClassMap<InstrumentClassMap>();
                csv.Context.RegisterClassMap<InstrumentPriceClassMap>();

                switch (templateType)
                {
                    case TemplateType.Portfolios:
                        var portfolioProcessor = _serviceProvider.GetRequiredService<PortfolioImportProcessor>();
                        var portfolioResult = await portfolioProcessor.ProcessImport(csv.GetRecords<PortfolioDto>());
                        SaveErrorLog(portfolioResult, logPath);
                        break;
                    case TemplateType.Positions:
                        var positionProcessor = _serviceProvider.GetRequiredService<PositionImportProcessor>();
                        var positionResult = await positionProcessor.ProcessImport(csv.GetRecords<PositionDto>());
                        SaveErrorLog(positionResult, logPath);
                        break;
                    case TemplateType.Instruments:
                        var instrumentProcessor = _serviceProvider.GetRequiredService<InstrumentImportProcessor>();
                        var instrumentResult = await instrumentProcessor.ProcessImport(csv.GetRecords<InstrumentDto>());
                        SaveErrorLog(instrumentResult, logPath);
                        break;
                    case TemplateType.Prices:
                        var priceProcessor = _serviceProvider.GetRequiredService<PriceImportProcessor>();
                        var pricesResult = await priceProcessor.ProcessImport(csv.GetRecords<InstrumentPriceDto>());
                        SaveErrorLog(pricesResult, logPath);
                        break;
                    case TemplateType.Transactions:
                        var transactionProcessor = _serviceProvider.GetRequiredService<TransactionImportProcessor>();
                        var transactionsResult =
                            await transactionProcessor.ProcessImport(csv.GetRecords<TransactionDto>());
                        SaveErrorLog(transactionsResult, logPath);
                        break;
                    default:
                        break;
                }

                importEntry.ChangeStatus(ImportStatus.Finished);
                importEntry.AddErrorLog(logPath);
                _importRepository.Update(importEntry);
                await _importRepository.UnitOfWork.CommitAsync();
            }
            catch (CsvHelperException ex)
            {
                _logger.LogError(ex.Message);
                importEntry.ChangeStatus(ImportStatus.Error, $"Error: failed to process received data as {templateType}.");
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

        private void SaveErrorLog<T>(ImportResult<T> importResult, string filename)
        {
            using var sw = new StreamWriter(filename);
            using var csv = new CsvWriter(sw, _csvConfig);
            csv.Context.RegisterClassMap<PortfolioClassMap>();
            csv.Context.RegisterClassMap<PositionImportClassMap>();
            csv.Context.RegisterClassMap<TransactionClassMap>();
            csv.Context.RegisterClassMap<InstrumentClassMap>();
            csv.Context.RegisterClassMap<InstrumentPriceClassMap>();

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
