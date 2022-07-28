using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Application.Services.Interfaces;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Services.Queries;

namespace PortEval.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionService _positionService;
        private readonly IPositionQueries _positionQueries;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PositionsController(IPositionService positionService, IPositionQueries positionQueries, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _positionService = positionService;
            _positionQueries = positionQueries;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger(typeof(PositionsController));
        }

        // GET api/positions/5
        [HttpGet("{positionId}")]
        public async Task<ActionResult<PositionDto>> GetPosition(int positionId)
        {
            _logger.LogInformation($"Position {positionId} requested.");

            var position = await _positionQueries.GetPosition(positionId);
            if (position.Status == QueryStatus.NotFound)
            {
                return NotFound($"Position {positionId} not found.");
            }

            return position.Response;
        }

        // GET api/positions/5/value?at=2021-01-01T00:00:00Z
        [HttpGet("{positionId}/value")]
        public async Task<ActionResult<EntityValueDto>> GetPositionValue(int positionId,
            [FromQuery] DateTime? at)
        {
            var time = at ?? DateTime.Now;
            _logger.LogInformation($"Position {positionId} value at {time} requested.");

            var value = await _positionQueries.GetPositionValue(positionId, time);
            if (value.Status == QueryStatus.NotFound)
            {
                return NotFound($"Position {positionId} not found.");
            }

            return value.Response;
        }

        // GET api/positions/5/profit?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z
        [HttpGet("{positionId}/profit")]
        public async Task<ActionResult<EntityProfitDto>> GetPositionProfit(int positionId, [FromQuery] DateRangeParams dateRange)
        {
            _logger.LogInformation($"Position {positionId} profit between {dateRange.From} and {dateRange.To} requested.");

            var profit = await _positionQueries.GetPositionProfit(positionId, dateRange);
            if (profit.Status == QueryStatus.NotFound)
            {
                return NotFound($"Position {positionId} not found.");
            }

            return profit.Response;
        }

        // GET api/positions/5/performance?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z
        [HttpGet("{positionId}/performance")]
        public async Task<ActionResult<EntityPerformanceDto>> GetPositionPerformance(int positionId, [FromQuery] DateRangeParams dateRange)
        {
            _logger.LogInformation($"Position {positionId} performance between {dateRange.From} and {dateRange.To} requested.");

            var performance = await _positionQueries.GetPositionPerformance(positionId, dateRange);
            if (performance.Status == QueryStatus.NotFound)
            {
                return NotFound($"Position {positionId} not found.");
            }

            return performance.Response;
        }

        // GET api/positions/5/value/chart?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z&frequency=day
        [HttpGet("{positionId}/value/chart")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPositionChartedValue(int positionId,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency, [FromQuery] string currency = null)
        {
            _logger.LogInformation($"Position {positionId} value chart between {dateRange.From} and {dateRange.To} with interval {frequency} requested.");

            var result = await _positionQueries.ChartPositionValue(positionId, dateRange, frequency, currency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Position {positionId} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/positions/5/profit/chart?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z&frequency=day
        [HttpGet("{positionId}/profit/chart")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPositionChartedProfit(int positionId,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency, [FromQuery] string currency = null)
        {
            _logger.LogInformation($"Position {positionId} profit chart between {dateRange.From} and {dateRange.To} with interval {frequency} requested.");

            var result = await _positionQueries.ChartPositionProfit(positionId, dateRange, frequency, currency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Position {positionId} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/positions/5/performance/chart?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z&frequency=day
        [HttpGet("{positionId}/performance/chart")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPositionChartedPerformance(int positionId,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency)
        {
            _logger.LogInformation($"Position {positionId} performance chart between {dateRange.From} and {dateRange.To} with interval {frequency} requested.");

            var result = await _positionQueries.ChartPositionPerformance(positionId, dateRange, frequency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Position {positionId} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/positions/5/profit/chart/aggregated?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z&frequency=day
        [HttpGet("{positionId}/profit/chart/aggregated")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPositionAggregatedProfit(int positionId,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency, [FromQuery] string currency = null)
        {
            _logger.LogInformation($"Position {positionId} aggregated profit chart between {dateRange.From} and {dateRange.To} with interval {frequency} requested.");

            var result = await _positionQueries.ChartPositionProfitAggregated(positionId, dateRange, frequency, currency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Position {positionId} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/positions/5/performance/chart/aggregated?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z&frequency=day
        [HttpGet("{positionId}/performance/chart/aggregated")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPositionAggregatedPerformance(int positionId,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency)
        {
            _logger.LogInformation($"Position {positionId} aggregated performance chart between {dateRange.From} and {dateRange.To} with interval {frequency} requested.");

            var result = await _positionQueries.ChartPositionPerformanceAggregated(positionId, dateRange, frequency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Position {positionId} not found.");
            }

            return result.Response.ToList();
        }

        [HttpGet("{positionId}/stats")]
        public async Task<ActionResult<PositionStatisticsDto>> GetPositionStatistics(int positionId)
        {
            _logger.LogInformation($"Position {positionId} statistics requested.");

            var result = await _positionQueries.GetPositionStatistics(positionId);
            if(result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Position {positionId} not found");
            }

            return result.Response;
        }

        // POST api/positions
        [HttpPost]
        public async Task<ActionResult<PositionDto>> PostPosition([FromBody] PositionDto createRequest)
        {
            _logger.LogInformation($"Creating position for portfolio {createRequest.PortfolioId}.");

            var createdPosition =
                await _positionService.OpenPositionAsync(createRequest);
            return CreatedAtAction("GetPosition", new { positionId = createdPosition.Id },
                _mapper.Map<PositionDto>(createdPosition));
        }

        // PUT api/positions/5
        [HttpPut("{positionId}")]
        public async Task<ActionResult<PositionDto>> PutPosition(int positionId, [FromBody] PositionDto updateRequest)
        {
            _logger.LogInformation($"Updating position {positionId}.");

            if (updateRequest.Id != positionId)
            {
                return BadRequest("URL position id and request body position id don't match.");
            }

            var updatedPosition =
                await _positionService.UpdatePositionAsync(updateRequest);

            return _mapper.Map<PositionDto>(updatedPosition);
        }

        // DELETE api/positions/5
        [HttpDelete("{positionId}")]
        public async Task<IActionResult> DeletePosition(int positionId)
        {
            _logger.LogInformation($"Deleting position {positionId}.");

            await _positionService.RemovePositionAsync(positionId);
            return Ok();
        }
    }
}
