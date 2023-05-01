using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Controllers;

[Route("portfolios")]
[ApiController]
public class PortfoliosController : PortEvalControllerBase
{
    private readonly IPortfolioService _portfolioService;
    private readonly IPositionService _positionService;

    public PortfoliosController(IPortfolioService portfolioService, IPositionService positionService)
    {
        _portfolioService = portfolioService;
        _positionService = positionService;
    }

    // GET: api/portfolios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PortfolioDto>>> GetPortfolios()
    {
        var portfolios = await _portfolioService.GetAllPortfoliosAsync();
        return GenerateActionResult(portfolios);
    }

    // GET api/portfolios/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PortfolioDto>> GetPortfolio(int id)
    {
        var portfolio = await _portfolioService.GetPortfolioAsync(id);
        return GenerateActionResult(portfolio);
    }

    // GET api/portfolios/5/positions
    [HttpGet("{id}/positions")]
    public async Task<ActionResult<IEnumerable<PositionDto>>> GetPositions(int id)
    {
        var positions = await _positionService.GetPortfolioPositionsAsync(id);
        return GenerateActionResult(positions);
    }

    // GET api/portfolios/5/positions/stats
    [HttpGet("{id}/positions/stats")]
    public async Task<ActionResult<IEnumerable<PositionStatisticsDto>>> GetPositionsStatistics(int id)
    {
        var result = await _positionService.GetPortfolioPositionsStatisticsAsync(id);
        return GenerateActionResult(result);
    }

    // GET api/portfolios/5/value?at=2021-01-01T02:00:00Z
    [HttpGet("{id}/value")]
    public async Task<ActionResult<EntityValueDto>> GetPortfolioValue(int id, [FromQuery] DateTime? at)
    {
        var time = at ?? DateTime.UtcNow;

        var value = await _portfolioService.GetPortfolioValueAsync(id, time);
        return GenerateActionResult(value);
    }

    // GET api/portfolios/5/profit?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z
    [HttpGet("{id}/profit")]
    public async Task<ActionResult<EntityProfitDto>> GetPortfolioProfit(int id, [FromQuery] DateRangeParams dateRange)
    {
        var profit = await _portfolioService.GetPortfolioProfitAsync(id, dateRange);
        return GenerateActionResult(profit);
    }

    // GET api/portfolios/5/performance?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z
    [HttpGet("{id}/performance")]
    public async Task<ActionResult<EntityPerformanceDto>> GetPortfolioPerformance(int id,
        [FromQuery] DateRangeParams dateRange)
    {
        var performance = await _portfolioService.GetPortfolioPerformanceAsync(id, dateRange);
        return GenerateActionResult(performance);
    }

    // GET api/portfolios/5/value/chart?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z&frequency=day
    [HttpGet("{id}/value/chart")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPortfolioChartedValue(int id,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency,
        [FromQuery] string currency = null)
    {
        var result = await _portfolioService.ChartPortfolioValueAsync(id, dateRange, frequency, currency);
        return GenerateActionResult(result);
    }

    // GET api/portfolios/5/profit/chart?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z&frequency=day
    [HttpGet("{id}/profit/chart")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPortfolioChartedProfit(int id,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency,
        [FromQuery] string currency = null)
    {
        var result = await _portfolioService.ChartPortfolioProfitAsync(id, dateRange, frequency, currency);
        return GenerateActionResult(result);
    }

    // GET api/portfolios/5/performance/chart?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z&frequency=day
    [HttpGet("{id}/performance/chart")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPortfolioChartedPerformance(int id,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency)
    {
        var result = await _portfolioService.ChartPortfolioPerformanceAsync(id, dateRange, frequency);
        return GenerateActionResult(result);
    }

    // GET api/portfolios/5/profit/chart/aggregated?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z&frequency=day
    [HttpGet("{id}/profit/chart/aggregated")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPortfolioAggregatedProfit(int id,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency,
        [FromQuery] string currency = null)
    {
        var result = await _portfolioService.ChartPortfolioAggregatedProfitAsync(id, dateRange, frequency, currency);
        return GenerateActionResult(result);
    }

    // GET api/portfolios/5/performance/chart/aggregated?from=2021-01-01T00:00:00Z&to=2021-10-01T00:00:00Z&frequency=day
    [HttpGet("{id}/performance/chart/aggregated")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetPortfolioAggregatedPerformance(int id,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency)
    {
        var result = await _portfolioService.ChartPortfolioAggregatedPerformanceAsync(id, dateRange, frequency);
        return GenerateActionResult(result);
    }

    // GET api/portfolios/stats
    [HttpGet("stats")]
    public async Task<ActionResult<IEnumerable<EntityStatisticsDto>>> GetAllPortfoliosStatistics()
    {
        var result = await _portfolioService.GetAllPortfoliosStatisticsAsync();
        return GenerateActionResult(result);
    }

    // GET api/portfolios/5/stats
    [HttpGet("{id}/stats")]
    public async Task<ActionResult<EntityStatisticsDto>> GetPortfolioStatistics(int id)
    {
        var result = await _portfolioService.GetPortfolioStatisticsAsync(id);
        return GenerateActionResult(result);
    }

    // POST api/portfolios
    [HttpPost]
    public async Task<ActionResult<PortfolioDto>> PostPortfolio([FromBody] PortfolioDto createRequest)
    {
        var createdPortfolio = await _portfolioService.CreatePortfolioAsync(createRequest);
        return GenerateActionResult(createdPortfolio, nameof(GetPortfolio), new { id = createdPortfolio.Response.Id });
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
        return GenerateActionResult(updatedPortfolio);
    }

    // DELETE api/portfolios/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePortfolio(int id)
    {
        var response = await _portfolioService.DeletePortfolioAsync(id);
        return GenerateActionResult(response);
    }
}