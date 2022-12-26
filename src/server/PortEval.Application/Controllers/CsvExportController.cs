using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.QueryParams;
using System;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("export")]
    [ApiController]
    public class CsvExportController : ControllerBase
    {
        private readonly IInstrumentQueries _instrumentQueries;
        private readonly IPortfolioQueries _portfolioQueries;
        private readonly IPositionQueries _positionQueries;
        private readonly ITransactionQueries _transactionQueries;
        private readonly ICsvExportService _exportService;

        public CsvExportController(IInstrumentQueries instrumentQueries, IPortfolioQueries portfolioQueries,
            IPositionQueries positionQueries, ITransactionQueries transactionQueries, ICsvExportService exportService)
        {
            _instrumentQueries = instrumentQueries;
            _portfolioQueries = portfolioQueries;
            _positionQueries = positionQueries;
            _transactionQueries = transactionQueries;
            _exportService = exportService;
        }

        [HttpGet("portfolios")]
        public async Task<IActionResult> GetPortfoliosExport()
        {
            var portfolios = await _portfolioQueries.GetPortfolios();

            var data = _exportService.ConvertToCsv(portfolios.Response);
            return File(data, "text/csv", GenerateCsvFileName("portfolios"));
        }

        [HttpGet("positions")]
        public async Task<IActionResult> GetPositionsExport()
        {
            var positions = await _positionQueries.GetAllPositions();

            var data = _exportService.ConvertToCsv(positions.Response);
            return File(data, "text/csv", GenerateCsvFileName("positions"));
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactionsExport([FromQuery] DateRangeParams dateRange)
        {
            var transactions = await _transactionQueries.GetTransactions(new TransactionFilters(), dateRange);

            var data = _exportService.ConvertToCsv(transactions.Response);
            return File(data, "text/csv", GenerateCsvFileName("transactions"));
        }

        [HttpGet("instruments")]
        public async Task<IActionResult> GetInstrumentExport()
        {
            var instruments = await _instrumentQueries.GetAllInstruments();

            var data = _exportService.ConvertToCsv(instruments.Response);
            return File(data, "text/csv", GenerateCsvFileName("instruments"));
        }

        [HttpGet("instruments/{instrumentId}/prices")]
        public async Task<IActionResult> GetPricesExport(int instrumentId, [FromQuery] DateRangeParams dateRange)
        {
            var prices = await _instrumentQueries.GetInstrumentPrices(instrumentId, dateRange);

            var data = _exportService.ConvertToCsv(prices.Response);
            return File(data, "text/csv", GenerateCsvFileName($"prices_${instrumentId}"));
        }

        private string GenerateCsvFileName(string prefix)
        {
            return $"{prefix}_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        }
    }
}
