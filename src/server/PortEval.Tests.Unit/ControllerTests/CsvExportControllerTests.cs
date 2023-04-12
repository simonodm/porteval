using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PortEval.Application.Controllers;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Tests.Unit.Helpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PortEval.Tests.Unit.ControllerTests
{
    public class CsvExportControllerTests
    {
        private IFixture _fixture;
        private Mock<IPortfolioService> _portfolioService;
        private Mock<IPositionService> _positionService;
        private Mock<ITransactionService> _transactionService;
        private Mock<ICsvExportService> _csvService;
        private Mock<IInstrumentPriceService> _instrumentPriceService;
        private Mock<IInstrumentService> _instrumentService;

        public CsvExportControllerTests()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
            _portfolioService = _fixture.Freeze<Mock<IPortfolioService>>();
            _positionService = _fixture.Freeze<Mock<IPositionService>>();
            _transactionService = _fixture.Freeze<Mock<ITransactionService>>();
            _csvService = _fixture.Freeze<Mock<ICsvExportService>>();
            _instrumentPriceService = _fixture.Freeze<Mock<IInstrumentPriceService>>();
            _instrumentService = _fixture.Freeze<Mock<IInstrumentService>>();
        }

        [Fact]
        public async Task GetPortfoliosExport_ReturnsCsvFileWithPortfolioData()
        {
            var portfolios = _fixture.CreateMany<PortfolioDto>();
            _portfolioService
                .Setup(m => m.GetAllPortfoliosAsync())
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(portfolios));
            _csvService
                .Setup(m => m.ConvertToCsv(portfolios))
                .Returns(OperationResponseHelper.GenerateSuccessfulOperationResponse(Encoding.UTF8.GetBytes($"{portfolios.First().Id}")));

            var sut = _fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetPortfoliosExport();

            _portfolioService.Verify(m => m.GetAllPortfoliosAsync(), Times.Once());
            _csvService.Verify(m => m.ConvertToCsv(portfolios), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(portfolios.First().Id.ToString(), result);
        }

        [Fact]
        public async Task GetPositionsExport_ReturnsCsvFileWithPositionsData()
        {
            var positions = _fixture.CreateMany<PositionDto>();
            _positionService
                .Setup(m => m.GetAllPositionsAsync())
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(positions));
            _csvService
                .Setup(m => m.ConvertToCsv(positions))
                .Returns(OperationResponseHelper.GenerateSuccessfulOperationResponse(Encoding.UTF8.GetBytes($"{positions.First().Id}")));

            var sut = _fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetPositionsExport();

            _positionService.Verify(m => m.GetAllPositionsAsync(), Times.Once());
            _csvService.Verify(m => m.ConvertToCsv(positions), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(positions.First().Id.ToString(), result);
        }

        [Fact]
        public async Task GetTransactionsExport_ReturnsCsvFileWithTransactionsData()
        {
            var dateRange = _fixture.Create<DateRangeParams>();

            var transactions = _fixture.CreateMany<TransactionDto>();
            _transactionService
                .Setup(m => m.GetTransactionsAsync(It.IsAny<TransactionFilters>(), dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(transactions));
            _csvService
                .Setup(m => m.ConvertToCsv(transactions))
                .Returns(OperationResponseHelper.GenerateSuccessfulOperationResponse(Encoding.UTF8.GetBytes($"{transactions.First().Id}")));

            var sut = _fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetTransactionsExport(dateRange);

            _transactionService.Verify(m => m.GetTransactionsAsync(It.IsAny<TransactionFilters>(), dateRange), Times.Once());
            _csvService.Verify(m => m.ConvertToCsv(transactions), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(transactions.First().Id.ToString(), result);
        }

        [Fact]
        public async Task GetInstrumentExport_ReturnsCsvFileWithInstrumentsData()
        {
            var instruments = _fixture.CreateMany<InstrumentDto>();
            _instrumentService
                .Setup(m => m.GetAllInstrumentsAsync())
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(instruments));
            _csvService
                .Setup(m => m.ConvertToCsv(instruments))
                .Returns(OperationResponseHelper.GenerateSuccessfulOperationResponse(Encoding.UTF8.GetBytes($"{instruments.First().Id}")));

            var sut = _fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetInstrumentExport();

            _instrumentService.Verify(m => m.GetAllInstrumentsAsync(), Times.Once());
            _csvService.Verify(m => m.ConvertToCsv(instruments), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(instruments.First().Id.ToString(), result);
        }

        [Fact]
        public async Task GetPricesExport_ReturnsCsvFileWithPricesData()
        {
            var instrumentId = _fixture.Create<int>();
            var dateRange = _fixture.Create<DateRangeParams>();
            var prices = _fixture.CreateMany<InstrumentPriceDto>();

            _instrumentPriceService
                .Setup(m => m.GetInstrumentPricesAsync(instrumentId, dateRange))
                .ReturnsAsync(OperationResponseHelper.GenerateSuccessfulOperationResponse(prices));
            _csvService
                .Setup(m => m.ConvertToCsv(prices))
                .Returns(OperationResponseHelper.GenerateSuccessfulOperationResponse(Encoding.UTF8.GetBytes($"{prices.First().Id}")));

            var sut = _fixture.Build<CsvExportController>().OmitAutoProperties().Create();

            var result = await sut.GetPricesExport(instrumentId, dateRange);

            _instrumentPriceService.Verify(m => m.GetInstrumentPricesAsync(instrumentId, dateRange), Times.Once());
            _csvService.Verify(m => m.ConvertToCsv(prices), Times.Once());
            ControllerTestHelper.AssertFileContentEqual(prices.First().Id.ToString(), result);
        }
    }
}
