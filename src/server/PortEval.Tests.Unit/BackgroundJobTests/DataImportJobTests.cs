using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Moq;
using PortEval.Application.Core.BackgroundJobs;
using PortEval.Application.Core.Common.BulkImportExport;
using PortEval.Application.Core.Interfaces;
using PortEval.Application.Core.Interfaces.Repositories;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
using PortEval.Tests.Unit.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.BackgroundJobTests
{
    public class DataImportJobTests
    {
        private IFixture _fixture;
        private Mock<IDataImportRepository> _dataImportRepository;
        private Mock<IServiceProvider> _serviceProvider;
        private MockFileSystem _fileSystem;
        private string _storagePath = "/storage/";

        public DataImportJobTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));
            _dataImportRepository = _fixture.CreateDefaultDataImportRepositoryMock();
            _serviceProvider = GetServiceProviderMockWithImportProcessors(_fixture);
            _fileSystem = _fixture.Freeze<MockFileSystem>();
        }

        [Fact]
        public async Task Run_ChangesImportStatusToFinished_WhenImportSucceeds()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            _fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note\n,ABC,USD,TestNote"));

            _dataImportRepository
                .Setup(m => m.FindAsync(dataImport.Id))
                .ReturnsAsync(dataImport);

            var sut = _fixture.Create<DataImportJob>();

            await sut.RunAsync(dataImport.Id, inputPath, logPath);

            _dataImportRepository.Verify(m => m.Update(It.Is<DataImport>(i => i.Id == dataImport.Id && i.Status == ImportStatus.Finished)));
        }

        [Fact]
        public async Task Run_ImportsPortfoliosFromProvidedFile_WhenPortfolioTemplateTypeIsSelected()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            _fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note\n,ABC,USD,TestNote"));

            _dataImportRepository
                .Setup(m => m.FindAsync(dataImport.Id))
                .ReturnsAsync(dataImport);

            var portfolioImportProcessor = GetImportProcessorMock<PortfolioDto>(_fixture);

            var sut = _fixture.Create<DataImportJob>();

            await sut.RunAsync(dataImport.Id, inputPath, logPath);

            portfolioImportProcessor.Verify(p => p.ImportRecordsAsync(
                It.Is<IEnumerable<PortfolioDto>>(e => e.Count() == 1 && e.Any(p => p.Name == "ABC" && p.CurrencyCode == "USD" && p.Note == "TestNote"))
            ));
        }

        [Fact]
        public async Task Run_ImportsPositionsFromProvidedFile_WhenPositionTemplateTypeIsSelected()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Positions);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            _fileSystem.AddFile(inputPath, new MockFileData("Position ID,Instrument ID,Portfolio ID,Note,Time,Amount,Price\n,1,1,TestNote,2022-01-01,5,100"));

            _dataImportRepository
                .Setup(m => m.FindAsync(dataImport.Id))
                .ReturnsAsync(dataImport);

            var positionImportProcessor = GetImportProcessorMock<PositionDto>(_fixture);

            var sut = _fixture.Create<DataImportJob>();

            await sut.RunAsync(dataImport.Id, inputPath, logPath);

            positionImportProcessor.Verify(p => p.ImportRecordsAsync(
                It.Is<IEnumerable<PositionDto>>(e =>
                    e.Count() == 1 &&
                    e.Any(p => p.InstrumentId == 1 && p.PortfolioId == 1 && p.Note == "TestNote" && p.Time == DateTime.Parse("2022-01-01") && p.Amount == 5m && p.Price == 100m)
            )));
        }

        [Fact]
        public async Task Run_ImportsTransactionsFromProvidedFile_WhenTransactionTemplateTypeIsSelected()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Transactions);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            _fileSystem.AddFile(inputPath, new MockFileData("Transaction ID,Position ID,Price,Amount,Time\n,1,100,-1,2005-04-12 14:59"));

            _dataImportRepository
                .Setup(m => m.FindAsync(dataImport.Id))
                .ReturnsAsync(dataImport);

            var transactionImportProcessor = GetImportProcessorMock<TransactionDto>(_fixture);

            var sut = _fixture.Create<DataImportJob>();

            await sut.RunAsync(dataImport.Id, inputPath, logPath);

            transactionImportProcessor.Verify(p => p.ImportRecordsAsync(
                It.Is<IEnumerable<TransactionDto>>(e =>
                    e.Count() == 1 &&
                    e.Any(t => t.PositionId == 1 && t.Price == 100m && t.Amount == -1m && t.Time == DateTime.Parse("2005-04-12 14:59"))
            )));
        }

        [Fact]
        public async Task Run_ImportsInstrumentsFromProvidedFile_WhenInstrumentTemplateTypeIsSelected()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Instruments);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            _fileSystem.AddFile(inputPath, new MockFileData("Instrument ID,Symbol,Name,Exchange,Type,Currency,Note\n1,AAPL,Apple Inc.,NASDAQ,stock,USD,TestNote"));

            _dataImportRepository
                .Setup(m => m.FindAsync(dataImport.Id))
                .ReturnsAsync(dataImport);

            var instrumentImportProcessor = GetImportProcessorMock<InstrumentDto>(_fixture);

            var sut = _fixture.Create<DataImportJob>();

            await sut.RunAsync(dataImport.Id, inputPath, logPath);

            instrumentImportProcessor.Verify(p => p.ImportRecordsAsync(
                It.Is<IEnumerable<InstrumentDto>>(e =>
                    e.Count() == 1 &&
                    e.Any(i => i.Id == 1 && i.Symbol == "AAPL" && i.Name == "Apple Inc." &&
                        i.Exchange == "NASDAQ" && i.Type == InstrumentType.Stock && i.Note == "TestNote"))
            ));
        }

        [Fact]
        public async Task Run_ImportsPricesFromProvidedFile_WhenPriceTemplateTypeIsSelected()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Prices);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            _fileSystem.AddFile(inputPath, new MockFileData("Price ID,Instrument ID,Price,Time\n0,14,39.99,2020-02-29"));

            _dataImportRepository
                .Setup(m => m.FindAsync(dataImport.Id))
                .ReturnsAsync(dataImport);

            var priceImportProcessor = GetImportProcessorMock<InstrumentPriceDto>(_fixture);

            var sut = _fixture.Create<DataImportJob>();

            await sut.RunAsync(dataImport.Id, inputPath, logPath);

            priceImportProcessor.Verify(p => p.ImportRecordsAsync(
                It.Is<IEnumerable<InstrumentPriceDto>>(e =>
                    e.Count() == 1 &&
                    e.Any(p => p.InstrumentId == 14 && p.Price == 39.99m && p.Time == DateTime.Parse("2020-02-29"))
            )));
        }

        [Fact]
        public async Task Run_ImportsZeroRecords_WhenNoDataRowsAreProvided()
        {

            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fileSystem = _fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note"));

            _dataImportRepository
                .Setup(m => m.FindAsync(dataImport.Id))
                .ReturnsAsync(dataImport);

            var portfolioImportProcessor = GetImportProcessorMock<PortfolioDto>(_fixture);

            var sut = _fixture.Create<DataImportJob>();

            await sut.RunAsync(dataImport.Id, inputPath, logPath);

            portfolioImportProcessor.Verify(p => p.ImportRecordsAsync(
                It.Is<IEnumerable<PortfolioDto>>(e => !e.Any())
            ));
        }

        [Fact]
        public async Task Run_ErrorLogContainsRowErrorMessage_WhenRowProcessingFails()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fakeErrorLogEntry = _fixture.Create<ProcessedRowErrorLogEntry<PortfolioDto>>();
            fakeErrorLogEntry.AddError("TestError");
            var fakeImportResult = _fixture
                .Build<ImportResult<PortfolioDto>>()
                .With(r => r.ErrorLog, new List<ProcessedRowErrorLogEntry<PortfolioDto>> { fakeErrorLogEntry })
                .Create();

            _fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note"));

            _dataImportRepository
                .Setup(m => m.FindAsync(dataImport.Id))
                .ReturnsAsync(dataImport);

            var portfolioImportProcessor = GetImportProcessorMock<PortfolioDto>(_fixture);
            portfolioImportProcessor
                .Setup(m => m.ImportRecordsAsync(It.IsAny<IEnumerable<PortfolioDto>>()))
                .ReturnsAsync(fakeImportResult);

            var sut = _fixture.Create<DataImportJob>();

            await sut.RunAsync(dataImport.Id, inputPath, logPath);

            var logFileLines = _fileSystem.File.ReadAllLines(logPath);
            var errorMessage = logFileLines[0].Split(',')[4];

            Assert.NotEmpty(errorMessage);
            Assert.NotEqual("OK", errorMessage);
        }

        [Fact]
        public async Task Run_ErrorLogContainsRowErrorMessage_WhenRowParsingFails()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            _fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note\n,abc,,"));

            _dataImportRepository
                .Setup(m => m.FindAsync(dataImport.Id))
                .ReturnsAsync(dataImport);

            var sut = _fixture.Create<DataImportJob>();

            await sut.RunAsync(dataImport.Id, inputPath, logPath);

            var logFileLines = _fileSystem.File.ReadAllLines(logPath);
            var errorMessage = logFileLines[0].Split(',')[4];

            Assert.NotEmpty(errorMessage);
            Assert.NotEqual("OK", errorMessage);
        }

        [Fact]
        public async Task Run_SetsImportStatusToError_WhenInvalidTemplateTypeIsProvided()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            _fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID;Note"));

            _dataImportRepository
                .Setup(m => m.FindAsync(dataImport.Id))
                .ReturnsAsync(dataImport);

            GetImportProcessorMock<PortfolioDto>(_fixture);

            var sut = _fixture.Create<DataImportJob>();

            await sut.RunAsync(dataImport.Id, inputPath, logPath);

            _dataImportRepository.Verify(r => r.Update(It.Is<DataImport>(d => d.Status == ImportStatus.Error && !string.IsNullOrEmpty(d.StatusDetails))));
        }

        [Fact]
        public async Task Run_SetsImportStatusToError_WhenImportThrows()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            _fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note"));

            _dataImportRepository
                .Setup(m => m.FindAsync(dataImport.Id))
                .ReturnsAsync(dataImport);

            var portfolioImportProcessor = GetImportProcessorMock<PortfolioDto>(_fixture);
            portfolioImportProcessor
                .Setup(m => m.ImportRecordsAsync(It.IsAny<IEnumerable<PortfolioDto>>()))
                .Throws<Exception>();

            var sut = _fixture.Create<DataImportJob>();

            await sut.RunAsync(dataImport.Id, inputPath, logPath);

            _dataImportRepository.Verify(r => r.Update(It.Is<DataImport>(d => d.Status == ImportStatus.Error && !string.IsNullOrEmpty(d.StatusDetails))));
        }

        [Fact]
        public async Task Run_DeletesInputFileAfterProcessing()
        {
            var dataImport = new DataImport(Guid.NewGuid(), DateTime.UtcNow, TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            _fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note"));
            _dataImportRepository
                .Setup(m => m.FindAsync(dataImport.Id))
                .ReturnsAsync(dataImport);

            var sut = _fixture.Create<DataImportJob>();

            await sut.RunAsync(dataImport.Id, inputPath, logPath);

            Assert.False(_fileSystem.FileExists(inputPath));
        }

        private static Mock<IServiceProvider> GetServiceProviderMockWithImportProcessors(IFixture fixture)
        {
            var portfolioImportProcessor = GetImportProcessorMock<PortfolioDto>(fixture);
            var positionImportProcessor = GetImportProcessorMock<PositionDto>(fixture);
            var transactionImportProcessor = GetImportProcessorMock<TransactionDto>(fixture);
            var instrumentImportProcessor = GetImportProcessorMock<InstrumentDto>(fixture);
            var priceImportProcessor = GetImportProcessorMock<InstrumentPriceDto>(fixture);

            var serviceProviderMock = fixture.Freeze<Mock<IServiceProvider>>();
            serviceProviderMock
                .Setup(m => m.GetService(typeof(IImportProcessor<PortfolioDto>)))
                .Returns(portfolioImportProcessor.Object);
            serviceProviderMock
                .Setup(m => m.GetService(typeof(IImportProcessor<PositionDto>)))
                .Returns(positionImportProcessor.Object);
            serviceProviderMock
                .Setup(m => m.GetService(typeof(IImportProcessor<TransactionDto>)))
                .Returns(transactionImportProcessor.Object);
            serviceProviderMock
                .Setup(m => m.GetService(typeof(IImportProcessor<InstrumentDto>)))
                .Returns(instrumentImportProcessor.Object);
            serviceProviderMock
                .Setup(m => m.GetService(typeof(IImportProcessor<InstrumentPriceDto>)))
                .Returns(priceImportProcessor.Object);

            return serviceProviderMock;
        }

        private static Mock<IImportProcessor<T>> GetImportProcessorMock<T>(IFixture fixture)
        {
            var mock = fixture.Freeze<Mock<IImportProcessor<T>>>();
            mock
                .Setup(m => m.ImportRecordsAsync(It.IsAny<IEnumerable<T>>()))
                .ReturnsAsync(fixture.Create<ImportResult<T>>);

            return mock;
        }
    }
}
