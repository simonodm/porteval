using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Tests.Unit.Helpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Services;
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
            var portfolioService = fixture.Freeze<Mock<IPortfolioService>>();
            portfolioService
                .Setup(m => m.GetAllPortfoliosAsync())
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(portfolios));
            var csvService = fixture.Freeze<Mock<ICsvExportService>>();
            csvService
                .Setup(m => m.ConvertToCsv(portfolios))
                .Returns(OperationResponseHelper.GenerateSuccessfulOperationResponse(Encoding.UTF8.GetBytes($"{portfolios.First().Id}")));

            var sut = fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfoliosExport();

            portfolioService.Verify(m => m.GetAllPortfoliosAsync(), Times.Once());
            csvService.Verify(m => m.ConvertToCsv(portfolios), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(portfolios.First().Id.ToString(), result);
        }

        [Fact]
        public async Task GetPositionsExport_ReturnsCsvFileWithPositionsData()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var positions = fixture.CreateMany<PositionDto>();
            var positionService = fixture.Freeze<Mock<IPositionService>>();
            positionService
                .Setup(m => m.GetAllPositionsAsync())
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(positions));
            var csvService = fixture.Freeze<Mock<ICsvExportService>>();
            csvService
                .Setup(m => m.ConvertToCsv(positions))
                .Returns(OperationResponseHelper.GenerateSuccessfulOperationResponse(Encoding.UTF8.GetBytes($"{positions.First().Id}")));

            var sut = fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionsExport();

            positionService.Verify(m => m.GetAllPositionsAsync(), Times.Once());
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
            var transactionService = fixture.Freeze<Mock<ITransactionService>>();
            transactionService
                .Setup(m => m.GetTransactionsAsync(It.IsAny<TransactionFilters>(), dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(transactions));
            var csvService = fixture.Freeze<Mock<ICsvExportService>>();
            csvService
                .Setup(m => m.ConvertToCsv(transactions))
                .Returns(OperationResponseHelper.GenerateSuccessfulOperationResponse(Encoding.UTF8.GetBytes($"{transactions.First().Id}")));

            var sut = fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetTransactionsExport(dateRange);

            transactionService.Verify(m => m.GetTransactionsAsync(It.IsAny<TransactionFilters>(), dateRange), Times.Once());
            csvService.Verify(m => m.ConvertToCsv(transactions), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(transactions.First().Id.ToString(), result);
        }

        [Fact]
        public async Task GetInstrumentExport_ReturnsCsvFileWithInstrumentsData()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            var instruments = fixture.CreateMany<InstrumentDto>();
            var instrumentService = fixture.Freeze<Mock<IInstrumentService>>();
            instrumentService
                .Setup(m => m.GetAllInstrumentsAsync())
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(instruments));
            var csvService = fixture.Freeze<Mock<ICsvExportService>>();
            csvService
                .Setup(m => m.ConvertToCsv(instruments))
                .Returns(OperationResponseHelper.GenerateSuccessfulOperationResponse(Encoding.UTF8.GetBytes($"{instruments.First().Id}")));

            var sut = fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentExport();

            instrumentService.Verify(m => m.GetAllInstrumentsAsync(), Times.Once());
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

            var instrumentPriceService = fixture.Freeze<Mock<IInstrumentPriceService>>();

            instrumentPriceService
                .Setup(m => m.GetInstrumentPricesAsync(instrumentId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(prices));
            var csvService = fixture.Freeze<Mock<ICsvExportService>>();
            csvService
                .Setup(m => m.ConvertToCsv(prices))
                .Returns(OperationResponseHelper.GenerateSuccessfulOperationResponse(Encoding.UTF8.GetBytes($"{prices.First().Id}")));

            var sut = fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetPricesExport(instrumentId, dateRange);

            instrumentPriceService.Verify(m => m.GetInstrumentPricesAsync(instrumentId, dateRange), Times.Once());
            csvService.Verify(m => m.ConvertToCsv(prices), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(prices.First().Id.ToString(), result);
        }
    }
}
