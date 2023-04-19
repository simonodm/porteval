using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Controllers;

[Route("positions")]
[ApiController]
public class PositionsController : PortEvalControllerBase
{
    private readonly IPositionService _positionService;

    public PositionsController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    // GET api/positions/5
    [HttpGet("{positionId}")]
    public async Task<ActionResult<PositionDto>> GetPosition(int positionId)
    {
        var position = await _positionService.GetPositionAsync(positionId);
        return GenerateActionResult(position);
    }

    // GET api/positions/5/value?at=2021-01-01T00:00:00Z
    [HttpGet("{positionId}/value")]
    public async Task<ActionResult<EntityValueDto>> GetPositionValue(int positionId,
        [FromQuery] DateTime? at)
    {
        var time = at ?? DateTime.UtcNow;

        var value = await _positionService.GetPositionValueAsync(positionId, time);
        return GenerateActionResult(value);
    }

    // GET api/positions/5/profit?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z
    [HttpGet("{positionId}/profit")]
    public async Task<ActionResult<EntityProfitDto>> GetPositionProfit(int positionId,
        [FromQuery] DateRangeParams dateRange)
    {
        var profit = await _positionService.GetPositionProfitAsync(positionId, dateRange);
        return GenerateActionResult(profit);
    }

    // GET api/positions/5/performance?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z
    [HttpGet("{positionId}/performance")]
    public async Task<ActionResult<EntityPerformanceDto>> GetPositionPerformance(int positionId,
        [FromQuery] DateRangeParams dateRange)
    {
        var performance = await _positionService.GetPositionPerformanceAsync(positionId, dateRange);
        return GenerateActionResult(performance);
    }

    // GET api/positions/5/bep?at=2022-01-01T00:00:00Z
    [HttpGet("{positionId}/bep")]
    public async Task<ActionResult<PositionBreakEvenPointDto>> GetPositionBreakEvenPoint(int positionId,
        [FromQuery] DateTime? at)
    {
        var time = at ?? DateTime.UtcNow;

        var breakEvenPoint = await _positionService.GetPositionBreakEvenPointAsync(positionId, time);
        return GenerateActionResult(breakEvenPoint);
    }

    // GET api/positions/5/value/chart?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z&frequency=day
    [HttpGet("{positionId}/value/chart")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPositionChartedValue(int positionId,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency,
        [FromQuery] string currency = null)
    {
        var result = await _positionService.ChartPositionValueAsync(positionId, dateRange, frequency, currency);
        return GenerateActionResult(result);
    }

    // GET api/positions/5/profit/chart?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z&frequency=day
    [HttpGet("{positionId}/profit/chart")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPositionChartedProfit(int positionId,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency,
        [FromQuery] string currency = null)
    {
        var result = await _positionService.ChartPositionProfitAsync(positionId, dateRange, frequency, currency);
        return GenerateActionResult(result);
    }

    // GET api/positions/5/performance/chart?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z&frequency=day
    [HttpGet("{positionId}/performance/chart")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPositionChartedPerformance(int positionId,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency)
    {
        var result = await _positionService.ChartPositionPerformanceAsync(positionId, dateRange, frequency);
        return GenerateActionResult(result);
    }

    // GET api/positions/5/profit/chart/aggregated?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z&frequency=day
    [HttpGet("{positionId}/profit/chart/aggregated")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPositionAggregatedProfit(int positionId,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency,
        [FromQuery] string currency = null)
    {
        var result =
            await _positionService.ChartPositionAggregatedProfitAsync(positionId, dateRange, frequency, currency);
        return GenerateActionResult(result);
    }

    // GET api/positions/5/performance/chart/aggregated?from=2021-01-01T00:00:00Z&to=2021-02-01T00:00:00Z&frequency=day
    [HttpGet("{positionId}/performance/chart/aggregated")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPositionAggregatedPerformance(int positionId,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency)
    {
        var result = await _positionService.ChartPositionAggregatedPerformanceAsync(positionId, dateRange, frequency);
        return GenerateActionResult(result);
    }

    [HttpGet("{positionId}/stats")]
    public async Task<ActionResult<PositionStatisticsDto>> GetPositionStatistics(int positionId)
    {
        var result = await _positionService.GetPositionStatisticsAsync(positionId);
        return GenerateActionResult(result);
    }

    // POST api/positions
    [HttpPost]
    public async Task<ActionResult<PositionDto>> PostPosition([FromBody] PositionDto createRequest)
    {
        var createdPosition =
            await _positionService.OpenPositionAsync(createRequest);
        return GenerateActionResult(createdPosition, nameof(GetPosition),
            new { positionId = createdPosition.Response.Id });
    }

    // PUT api/positions/5
    [HttpPut("{positionId}")]
    public async Task<ActionResult<PositionDto>> PutPosition(int positionId, [FromBody] PositionDto updateRequest)
    {
        if (updateRequest.Id != positionId)
            return BadRequest("URL position id and request body position id don't match.");

        var updatedPosition = await _positionService.UpdatePositionAsync(updateRequest);
        return GenerateActionResult(updatedPosition);
    }

    // DELETE api/positions/5
    [HttpDelete("{positionId}")]
    public async Task<IActionResult> DeletePosition(int positionId)
    {
        var response = await _positionService.RemovePositionAsync(positionId);
        return GenerateActionResult(response);
    }
}