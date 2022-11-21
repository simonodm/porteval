using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Tests.Unit.Helpers;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class CsvExportControllerTests
    {
        [Fact]
        public async Task GetPortfoliosExport_ReturnsCsvFileWithPortfolioData()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var portfolios = fixture.CreateMany<PortfolioDto>();
            var portfolioQueries = fixture.Freeze<Mock<IPortfolioQueries>>();
            portfolioQueries
                .Setup(m => m.GetPortfolios())
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(portfolios));
            var csvService = fixture.Freeze<Mock<ICsvExportService>>();
            csvService
                .Setup(m => m.ConvertToCsv(portfolios))
                .Returns(Encoding.UTF8.GetBytes($"{portfolios.First().Id}"));

            var sut = fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfoliosExport();

            portfolioQueries.Verify(m => m.GetPortfolios(), Times.Once());
            csvService.Verify(m => m.ConvertToCsv(portfolios), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(portfolios.First().Id.ToString(), result);
        }

        [Fact]
        public async Task GetPositionsExport_ReturnsCsvFileWithPositionsData()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positions = fixture.CreateMany<PositionDto>();
            var positionQueries = fixture.Freeze<Mock<IPositionQueries>>();
            positionQueries
                .Setup(m => m.GetAllPositions())
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(positions));
            var csvService = fixture.Freeze<Mock<ICsvExportService>>();
            csvService
                .Setup(m => m.ConvertToCsv(positions))
                .Returns(Encoding.UTF8.GetBytes($"{positions.First().Id}"));

            var sut = fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionsExport();

            positionQueries.Verify(m => m.GetAllPositions(), Times.Once());
            csvService.Verify(m => m.ConvertToCsv(positions), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(positions.First().Id.ToString(), result);
        }

        [Fact]
        public async Task GetTransactionsExport_ReturnsCsvFileWithTransactionsData()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var dateRange = fixture.Create<DateRangeParams>();

            var transactions = fixture.CreateMany<TransactionDto>();
            var transactionQueries = fixture.Freeze<Mock<ITransactionQueries>>();
            transactionQueries
                .Setup(m => m.GetTransactions(It.IsAny<TransactionFilters>(), dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(transactions));
            var csvService = fixture.Freeze<Mock<ICsvExportService>>();
            csvService
                .Setup(m => m.ConvertToCsv(transactions))
                .Returns(Encoding.UTF8.GetBytes($"{transactions.First().Id}"));

            var sut = fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetTransactionsExport(dateRange);

            transactionQueries.Verify(m => m.GetTransactions(It.IsAny<TransactionFilters>(), dateRange), Times.Once());
            csvService.Verify(m => m.ConvertToCsv(transactions), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(transactions.First().Id.ToString(), result);
        }

        [Fact]
        public async Task GetInstrumentExport_ReturnsCsvFileWithInstrumentsData()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instruments = fixture.CreateMany<InstrumentDto>();
            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();
            instrumentQueries
                .Setup(m => m.GetAllInstruments())
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(instruments));
            var csvService = fixture.Freeze<Mock<ICsvExportService>>();
            csvService
                .Setup(m => m.ConvertToCsv(instruments))
                .Returns(Encoding.UTF8.GetBytes($"{instruments.First().Id}"));

            var sut = fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentExport();

            instrumentQueries.Verify(m => m.GetAllInstruments(), Times.Once());
            csvService.Verify(m => m.ConvertToCsv(instruments), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(instruments.First().Id.ToString(), result);
        }

        [Fact]
        public async Task GetPricesExport_ReturnsCsvFileWithPricesData()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instrumentId = fixture.Create<int>();
            var dateRange = fixture.Create<DateRangeParams>();
            var prices = fixture.CreateMany<InstrumentPriceDto>();

            var instrumentQueries = fixture.Freeze<Mock<IInstrumentQueries>>();

            instrumentQueries
                .Setup(m => m.GetInstrumentPrices(instrumentId, dateRange))
                .ReturnsAsync(ControllerTestHelper.GenerateSuccessfulQueryResponse(prices));
            var csvService = fixture.Freeze<Mock<ICsvExportService>>();
            csvService
                .Setup(m => m.ConvertToCsv(prices))
                .Returns(Encoding.UTF8.GetBytes($"{prices.First().Id}"));

            var sut = fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetPricesExport(instrumentId, dateRange);

            instrumentQueries.Verify(m => m.GetInstrumentPrices(instrumentId, dateRange), Times.Once());
            csvService.Verify(m => m.ConvertToCsv(prices), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(prices.First().Id.ToString(), result);
        }
    }
}
