using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Features.Queries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PortfoliosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPortfolioService _portfolioService;
        private readonly IPortfolioQueries _portfolioQueries;
        private readonly IPositionQueries _positionQueries;

        public PortfoliosController(IMapper mapper,
            IPortfolioService portfolioService, IPortfolioQueries portfolioQueries, IPositionQueries positionQueries)
        {
            _mapper = mapper;
            _portfolioService = portfolioService;
            _portfolioQueries = portfolioQueries;
            _positionQueries = positionQueries;
        }

        // GET: api/portfolios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PortfolioDto>>> GetPortfolios()
        {
            var portfolios = await _portfolioQueries.GetPortfolios();
            return portfolios.Response.ToList();
        }

        // GET api/portfolios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PortfolioDto>> GetPortfolio(int id)
        {
            var portfolio = await _portfolioQueries.GetPortfolio(id);
            if (portfolio.Status == QueryStatus.NotFound)
            {
                return NotFound($"Portfolio {id} not found.");
            }

            return portfolio.Response;
        }

        // GET api/portfolios/5/positions
        [HttpGet("{id}/positions")]
        public async Task<ActionResult<IEnumerable<PositionDto>>> GetPositions(int id)
        {
            var positions = await _positionQueries.GetPortfolioPositions(id);
            if (positions.Status == QueryStatus.NotFound)
            {
                return NotFound($"Portfolio {id} not found.");
            }

            return positions.Response.ToList();
        }

        // GET api/portfolios/5/positions/stats
        [HttpGet("{id}/positions/stats")]
        public async Task<ActionResult<IEnumerable<PositionStatisticsDto>>> GetPositionsStatistics(int id)
        {
            var result = await _positionQueries.GetPortfolioPositionsStatistics(id);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Portfolio {id} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/portfolios/5/value?at=2021-01-01T02:00:00Z
        [HttpGet("{id}/value")]
        public async Task<ActionResult<EntityValueDto>> GetPortfolioValue(int id, [FromQuery] DateTime? at)
        {
            var time = at ?? DateTime.UtcNow;

            var value = await _portfolioQueries.GetPortfolioValue(id, time);
            if (value.Status == QueryStatus.NotFound)
            {
                return NotFound($"Portfolio {id} not found.");
            }

            return value.Response;
        }

        // GET api/portfolios/5/profit?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z
        [HttpGet("{id}/profit")]
        public async Task<ActionResult<EntityProfitDto>> GetPortfolioProfit(int id, [FromQuery] DateRangeParams dateRange)
        {
            var profit = await _portfolioQueries.GetPortfolioProfit(id, dateRange);
            if (profit.Status == QueryStatus.NotFound)
            {
                return NotFound($"Portfolio {id} not found.");
            }

            return profit.Response;
        }

        // GET api/portfolios/5/performance?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z
        [HttpGet("{id}/performance")]
        public async Task<ActionResult<EntityPerformanceDto>> GetPortfolioPerformance(int id, [FromQuery] DateRangeParams dateRange)
        {
            var performance = await _portfolioQueries.GetPortfolioPerformance(id, dateRange);
            if (performance.Status == QueryStatus.NotFound)
            {
                return NotFound($"Portfolio {id} not found.");
            }

            return performance.Response;
        }

        // GET api/portfolios/5/value/chart?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z&frequency=day
        [HttpGet("{id}/value/chart")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPortfolioChartedValue(int id,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency, [FromQuery] string currency = null)
        {
            var result = await _portfolioQueries.ChartPortfolioValue(id, dateRange, frequency, currency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Portfolio {id} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/portfolios/5/profit/chart?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z&frequency=day
        [HttpGet("{id}/profit/chart")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPortfolioChartedProfit(int id,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency, [FromQuery] string currency = null)
        {
            var result = await _portfolioQueries.ChartPortfolioProfit(id, dateRange, frequency, currency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Portfolio {id} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/portfolios/5/performance/chart?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z&frequency=day
        [HttpGet("{id}/performance/chart")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPortfolioChartedPerformance(int id,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency)
        {
            var result = await _portfolioQueries.ChartPortfolioPerformance(id, dateRange, frequency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Portfolio {id} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/portfolios/5/profit/chart/aggregated?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z&frequency=day
        [HttpGet("{id}/profit/chart/aggregated")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPortfolioAggregatedProfit(int id,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency, [FromQuery] string currency = null)
        {
            var result = await _portfolioQueries.ChartPortfolioProfitAggregated(id, dateRange, frequency, currency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Portfolio {id} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/portfolios/5/performance/chart/aggregated?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z&frequency=day
        [HttpGet("{id}/performance/chart/aggregated")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPortfolioAggregatedPerformance(int id,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency)
        {
            var result = await _portfolioQueries.ChartPortfolioPerformanceAggregated(id, dateRange, frequency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Portfolio {id} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/portfolios/stats
        [HttpGet("stats")]
        public async Task<ActionResult<IEnumerable<EntityStatisticsDto>>> GetAllPortfoliosStatistics()
        {
            var result = await _portfolioQueries.GetAllPortfoliosStatistics();

            return result.Response.ToList();
        }

        // GET api/portfolios/5/stats
        [HttpGet("{id}/stats")]
        public async Task<ActionResult<EntityStatisticsDto>> GetPortfolioStatistics(int id)
        {
            var result = await _portfolioQueries.GetPortfolioStatistics(id);

            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Portfolio {id} not found.");
            }

            return result.Response;
        }

        // POST api/portfolios
        [HttpPost]
        public async Task<ActionResult<PortfolioDto>> PostPortfolio([FromBody] PortfolioDto createRequest)
        {
            var createdPortfolio = await _portfolioService.CreatePortfolioAsync(createRequest);
            return CreatedAtAction("GetPortfolio", new { id = createdPortfolio.Id }, _mapper.Map<PortfolioDto>(createdPortfolio));
        }

        // PUT api/portfolios/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PortfolioDto>> PutPortfolio(int id, [FromBody] PortfolioDto updateRequest)
        {
            if (id != updateRequest.Id)
            {
                return BadRequest("URL portfolio id and request body id don't match.");
            }

            var updatedPortfolio = await _portfolioService.UpdatePortfolioAsync(updateRequest);

            return _mapper.Map<PortfolioDto>(updatedPortfolio);
        }

        // DELETE api/portfolios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePortfolio(int id)
        {
            await _portfolioService.DeletePortfolioAsync(id);
            return Ok();
        }
    }
}
