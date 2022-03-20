using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Application.Services.Extensions;
using PortEval.Application.Services.Interfaces;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using PortEval.Application.Services.BulkImportExport;
using PortEval.Application.Services.BulkImportExport.ClassMaps;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Services
{
    public class CsvImportService : ICsvImportService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CsvConfiguration _csvConfig;
        private readonly string _logPath;

        public CsvImportService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
            };
            _logPath = Environment.GetEnvironmentVariable("FILE_STORAGE");
        }

        public async Task ProcessUpload(Guid importId, Stream inputFileStream, CsvTemplateType templateType)
        {
            using var reader = new StreamReader(inputFileStream);
            using var csv = new CsvReader(reader, _csvConfig);
            csv.Context.RegisterClassMap<PortfolioClassMap>();
            csv.Context.RegisterClassMap<PositionClassMap>();
            csv.Context.RegisterClassMap<TransactionClassMap>();
            csv.Context.RegisterClassMap<InstrumentClassMap>();
            csv.Context.RegisterClassMap<InstrumentPriceClassMap>();

            switch (templateType)
            {
                case CsvTemplateType.Portfolios:
                    var portfolioProcessor = _serviceProvider.GetRequiredService<PortfolioImportProcessor>();
                    var portfolioResult = await portfolioProcessor.ProcessImport(csv.GetRecords<PortfolioDto>());
                    SaveErrorLog(portfolioResult, importId.ToString());
                    return;
                case CsvTemplateType.Positions:
                    var positionProcessor = _serviceProvider.GetRequiredService<PositionImportProcessor>();
                    var positionResult = await positionProcessor.ProcessImport(csv.GetRecords<PositionDto>());
                    SaveErrorLog(positionResult, importId.ToString());
                    return;
                case CsvTemplateType.Instruments:
                    var instrumentProcessor = _serviceProvider.GetRequiredService<InstrumentImportProcessor>();
                    var instrumentResult = await instrumentProcessor.ProcessImport(csv.GetRecords<InstrumentDto>());
                    SaveErrorLog(instrumentResult, importId.ToString());
                    return;
                case CsvTemplateType.Prices:
                    var priceProcessor = _serviceProvider.GetRequiredService<PriceImportProcessor>();
                    var pricesResult = await priceProcessor.ProcessImport(csv.GetRecords<InstrumentPriceDto>());
                    SaveErrorLog(pricesResult, importId.ToString());
                    return;
                case CsvTemplateType.Transactions:
                    var transactionProcessor = _serviceProvider.GetRequiredService<TransactionImportProcessor>();
                    var transactionsResult = await transactionProcessor.ProcessImport(csv.GetRecords<TransactionDto>());
                    SaveErrorLog(transactionsResult, importId.ToString());
                    return;
                default:
                    return;
            }
        }

        public Stream TryGetErrorLog(Guid guid)
        {
            try
            {
                return new FileStream(_logPath + '/' + guid + ".csv", FileMode.Open);
            }
            catch
            {
                return null;
            }
        }

        private void SaveErrorLog<T>(ImportResult<T> importResult, string filename)
        {
            if (_logPath == null)
            {
                return;
            }

            using var sw = new StreamWriter(_logPath + '/' + filename + ".csv");
            using var csv = new CsvWriter(sw, _csvConfig);
            csv.Context.RegisterClassMap<PortfolioClassMap>();
            csv.Context.RegisterClassMap<PositionClassMap>();
            csv.Context.RegisterClassMap<TransactionClassMap>();
            csv.Context.RegisterClassMap<InstrumentClassMap>();
            csv.Context.RegisterClassMap<InstrumentPriceClassMap>();

            csv.WriteErrorHeaders<T>();
            foreach (var entry in importResult.ErrorLog)
            {
                csv.WriteErrorEntry(entry);
            }
        }
    }
}
