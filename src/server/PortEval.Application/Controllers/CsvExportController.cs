using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.QueryParams;
using System;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("export")]
    [ApiController]
    public class CsvExportController : PortEvalControllerBase
    {
        private readonly IInstrumentService _instrumentService;
        private readonly IInstrumentPriceService _priceService;
        private readonly IPortfolioService _portfolioService;
        private readonly IPositionService _positionService;
        private readonly ITransactionService _transactionService;
        private readonly ICsvExportService _exportService;

        public CsvExportController(IInstrumentService instrumentService, IInstrumentPriceService priceService, IPortfolioService portfolioService, IPositionService positionService, ITransactionService transactionService, ICsvExportService exportService)
        {
            _instrumentService = instrumentService;
            _priceService = priceService;
            _portfolioService = portfolioService;
            _positionService = positionService;
            _transactionService = transactionService;
            _exportService = exportService;
        }

        [HttpGet("portfolios")]
        public async Task<IActionResult> GetPortfoliosExport()
        {
            var portfolios = await _portfolioService.GetAllPortfoliosAsync();

            var data = _exportService.ConvertToCsv(portfolios.Response);
            return GenerateFileActionResult(data, "text/csv", GenerateCsvFileName("portfolios"));
        }

        [HttpGet("positions")]
        public async Task<IActionResult> GetPositionsExport()
        {
            var positions = await _positionService.GetAllPositionsAsync();

            var data = _exportService.ConvertToCsv(positions.Response);
            return GenerateFileActionResult(data, "text/csv", GenerateCsvFileName("positions"));
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactionsExport([FromQuery] DateRangeParams dateRange)
        {
            var transactions = await _transactionService.GetTransactionsAsync(new TransactionFilters(), dateRange);

            var data = _exportService.ConvertToCsv(transactions.Response);
            return GenerateFileActionResult(data, "text/csv", GenerateCsvFileName("transactions"));
        }

        [HttpGet("instruments")]
        public async Task<IActionResult> GetInstrumentExport()
        {
            var instruments = await _instrumentService.GetAllInstrumentsAsync();

            var data = _exportService.ConvertToCsv(instruments.Response);
            return GenerateFileActionResult(data, "text/csv", GenerateCsvFileName("instruments"));
        }

        [HttpGet("instruments/{instrumentId}/prices")]
        public async Task<IActionResult> GetPricesExport(int instrumentId, [FromQuery] DateRangeParams dateRange)
        {
            var prices = await _priceService.GetInstrumentPricesAsync(instrumentId, dateRange);

            var data = _exportService.ConvertToCsv(prices.Response);
            return GenerateFileActionResult(data, "text/csv", GenerateCsvFileName($"prices_${instrumentId}"));
        }

        private string GenerateCsvFileName(string prefix)
        {
            return $"{prefix}_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        }
    }
}
