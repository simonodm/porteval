using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Moq;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.BulkImportExport;
using PortEval.Application.Services.BulkImportExport.Interfaces;
using PortEval.Application.Services.Interfaces.Repositories;
using PortEval.BackgroundJobs.DataImport;
using PortEval.Domain.Models.Entities;
using PortEval.Domain.Models.Enums;
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
        private string _storagePath = "/storage/";

        [Fact]
        public async Task Run_ChangesImportStatusToFinished_WhenImportSucceeds()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            GetServiceProviderMockWithImportProcessors(fixture);

            var dataImport = new DataImport(Guid.NewGuid(), TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note\n,ABC,USD,TestNote"));

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);

            var sut = fixture.Create<DataImportJob>();

            await sut.Run(dataImport, inputPath, logPath);

            dataImportRepository.Verify(m => m.Update(It.Is<DataImport>(i => i.Id == dataImport.Id && i.Status == ImportStatus.Finished)));
        }

        [Fact]
        public async Task Run_ImportsPortfoliosFromProvidedFile_WhenPortfolioTemplateTypeIsSelected()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            GetServiceProviderMockWithImportProcessors(fixture);

            var dataImport = new DataImport(Guid.NewGuid(), TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note\n,ABC,USD,TestNote"));

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);

            var portfolioImportProcessor = GetImportProcessorMock<PortfolioDto>(fixture);

            var sut = fixture.Create<DataImportJob>();

            await sut.Run(dataImport, inputPath, logPath);

            portfolioImportProcessor.Verify(p => p.ImportRecords(
                It.Is<IEnumerable<PortfolioDto>>(e => e.Count() == 1 && e.Any(p => p.Name == "ABC" && p.CurrencyCode == "USD" && p.Note == "TestNote"))
            ));
        }

        [Fact]
        public async Task Run_ImportsPositionsFromProvidedFile_WhenPositionTemplateTypeIsSelected()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            GetServiceProviderMockWithImportProcessors(fixture);

            var dataImport = new DataImport(Guid.NewGuid(), TemplateType.Positions);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Position ID,Instrument ID,Portfolio ID,Note,Time,Amount,Price\n,1,1,TestNote,2022-01-01,5,100"));

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);

            var positionImportProcessor = GetImportProcessorMock<PositionDto>(fixture);

            var sut = fixture.Create<DataImportJob>();

            await sut.Run(dataImport, inputPath, logPath);

            positionImportProcessor.Verify(p => p.ImportRecords(
                It.Is<IEnumerable<PositionDto>>(e =>
                    e.Count() == 1 &&
                    e.Any(p => p.InstrumentId == 1 && p.PortfolioId == 1 && p.Note == "TestNote" && p.Time == DateTime.Parse("2022-01-01") && p.Amount == 5m && p.Price == 100m)
            )));
        }

        [Fact]
        public async Task Run_ImportsTransactionsFromProvidedFile_WhenTransactionTemplateTypeIsSelected()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            GetServiceProviderMockWithImportProcessors(fixture);

            var dataImport = new DataImport(Guid.NewGuid(), TemplateType.Transactions);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Transaction ID,Position ID,Price,Amount,Time\n,1,100,-1,2005-04-12 14:59"));

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);

            var transactionImportProcessor = GetImportProcessorMock<TransactionDto>(fixture);

            var sut = fixture.Create<DataImportJob>();

            await sut.Run(dataImport, inputPath, logPath);

            transactionImportProcessor.Verify(p => p.ImportRecords(
                It.Is<IEnumerable<TransactionDto>>(e =>
                    e.Count() == 1 &&
                    e.Any(t => t.PositionId == 1 && t.Price == 100m && t.Amount == -1m && t.Time == DateTime.Parse("2005-04-12 14:59"))
            )));
        }

        [Fact]
        public async Task Run_ImportsInstrumentsFromProvidedFile_WhenInstrumentTemplateTypeIsSelected()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            GetServiceProviderMockWithImportProcessors(fixture);

            var dataImport = new DataImport(Guid.NewGuid(), TemplateType.Instruments);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Instrument ID,Symbol,Name,Exchange,Type,Currency,Note\n1,AAPL,Apple Inc.,NASDAQ,stock,USD,TestNote"));

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);

            var instrumentImportProcessor = GetImportProcessorMock<InstrumentDto>(fixture);

            var sut = fixture.Create<DataImportJob>();

            await sut.Run(dataImport, inputPath, logPath);

            instrumentImportProcessor.Verify(p => p.ImportRecords(
                It.Is<IEnumerable<InstrumentDto>>(e =>
                    e.Count() == 1 &&
                    e.Any(i => i.Id == 1 && i.Symbol == "AAPL" && i.Name == "Apple Inc." &&
                        i.Exchange == "NASDAQ" && i.Type == InstrumentType.Stock && i.Note == "TestNote"))
            ));
        }

        [Fact]
        public async Task Run_ImportsPricesFromProvidedFile_WhenPriceTemplateTypeIsSelected()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            GetServiceProviderMockWithImportProcessors(fixture);

            var dataImport = new DataImport(Guid.NewGuid(), TemplateType.Prices);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Price ID,Instrument ID,Price,Time\n0,14,39.99,2020-02-29"));

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);

            var priceImportProcessor = GetImportProcessorMock<InstrumentPriceDto>(fixture);

            var sut = fixture.Create<DataImportJob>();

            await sut.Run(dataImport, inputPath, logPath);

            priceImportProcessor.Verify(p => p.ImportRecords(
                It.Is<IEnumerable<InstrumentPriceDto>>(e =>
                    e.Count() == 1 &&
                    e.Any(p => p.InstrumentId == 14 && p.Price == 39.99m && p.Time == DateTime.Parse("2020-02-29"))
            )));
        }

        [Fact]
        public async Task Run_ImportsZeroRecords_WhenNoDataRowsAreProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            GetServiceProviderMockWithImportProcessors(fixture);

            var dataImport = new DataImport(Guid.NewGuid(), TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note"));

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);

            var portfolioImportProcessor = GetImportProcessorMock<PortfolioDto>(fixture);

            var sut = fixture.Create<DataImportJob>();

            await sut.Run(dataImport, inputPath, logPath);

            portfolioImportProcessor.Verify(p => p.ImportRecords(
                It.Is<IEnumerable<PortfolioDto>>(e => !e.Any())
            ));
        }

        [Fact]
        public async Task Run_ErrorLogContainsRowErrorMessage_WhenRowProcessingFails()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            GetServiceProviderMockWithImportProcessors(fixture);

            var dataImport = new DataImport(Guid.NewGuid(), TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fakeErrorLogEntry = fixture.Create<ProcessedRowErrorLogEntry<PortfolioDto>>();
            fakeErrorLogEntry.AddError("TestError");
            var fakeImportResult = fixture
                .Build<ImportResult<PortfolioDto>>()
                .With(r => r.ErrorLog, new List<ProcessedRowErrorLogEntry<PortfolioDto>> { fakeErrorLogEntry })
                .Create();

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note"));

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);

            var portfolioImportProcessor = GetImportProcessorMock<PortfolioDto>(fixture);
            portfolioImportProcessor
                .Setup(m => m.ImportRecords(It.IsAny<IEnumerable<PortfolioDto>>()))
                .ReturnsAsync(fakeImportResult);

            var sut = fixture.Create<DataImportJob>();

            await sut.Run(dataImport, inputPath, logPath);

            var logFileLines = fileSystem.File.ReadAllLines(logPath);
            var errorMessage = logFileLines[0].Split(',')[4];

            Assert.NotEmpty(errorMessage);
            Assert.NotEqual("OK", errorMessage);
        }

        [Fact]
        public async Task Run_ErrorLogContainsRowErrorMessage_WhenRowParsingFails()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            GetServiceProviderMockWithImportProcessors(fixture);

            var dataImport = new DataImport(Guid.NewGuid(), TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note\n,abc,,"));

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);

            var sut = fixture.Create<DataImportJob>();

            await sut.Run(dataImport, inputPath, logPath);

            var logFileLines = fileSystem.File.ReadAllLines(logPath);
            var errorMessage = logFileLines[0].Split(',')[4];

            Assert.NotEmpty(errorMessage);
            Assert.NotEqual("OK", errorMessage);
        }

        [Fact]
        public async Task Run_SetsImportStatusToError_WhenInvalidTemplateTypeIsProvided()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            GetServiceProviderMockWithImportProcessors(fixture);

            var dataImport = new DataImport(Guid.NewGuid(), TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID;Note"));

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);

            var portfolioImportProcessor = GetImportProcessorMock<PortfolioDto>(fixture);

            var sut = fixture.Create<DataImportJob>();

            await sut.Run(dataImport, inputPath, logPath);

            dataImportRepository.Verify(r => r.Update(It.Is<DataImport>(d => d.Status == ImportStatus.Error && !string.IsNullOrEmpty(d.StatusDetails))));
        }

        [Fact]
        public async Task Run_SetsImportStatusToError_WhenImportThrows()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            GetServiceProviderMockWithImportProcessors(fixture);

            var dataImport = new DataImport(Guid.NewGuid(), TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note"));

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);

            var portfolioImportProcessor = GetImportProcessorMock<PortfolioDto>(fixture);
            portfolioImportProcessor
                .Setup(m => m.ImportRecords(It.IsAny<IEnumerable<PortfolioDto>>()))
                .Throws<Exception>();

            var sut = fixture.Create<DataImportJob>();

            await sut.Run(dataImport, inputPath, logPath);

            dataImportRepository.Verify(r => r.Update(It.Is<DataImport>(d => d.Status == ImportStatus.Error && !string.IsNullOrEmpty(d.StatusDetails))));
        }

        [Fact]
        public async Task Run_DeletesInputFileAfterProcessing()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            fixture.Customizations.Add(new TypeRelay(typeof(IFileSystem), typeof(MockFileSystem)));

            GetServiceProviderMockWithImportProcessors(fixture);

            var dataImport = new DataImport(Guid.NewGuid(), TemplateType.Portfolios);
            var inputPath = Path.Combine(_storagePath, "test.csv");
            var logPath = Path.Combine(_storagePath, "log.csv");

            var fileSystem = fixture.Freeze<MockFileSystem>();
            fileSystem.AddFile(inputPath, new MockFileData("Portfolio ID,Name,Currency,Note"));

            var dataImportRepository = fixture.Freeze<Mock<IDataImportRepository>>();
            dataImportRepository
                .Setup(m => m.Update(It.IsAny<DataImport>()))
                .Returns<DataImport>(d => d);

            var sut = fixture.Create<DataImportJob>();

            await sut.Run(dataImport, inputPath, logPath);

            Assert.False(fileSystem.FileExists(inputPath));
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
                .Setup(m => m.ImportRecords(It.IsAny<IEnumerable<T>>()))
                .ReturnsAsync(fixture.Create<ImportResult<T>>);

            return mock;
        }
    }
}
