using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Controllers;

[Route("instruments")]
[ApiController]
public class InstrumentsController : PortEvalControllerBase
{
    private readonly IInstrumentService _instrumentService;

    public InstrumentsController(IInstrumentService instrumentService)
    {
        _instrumentService = instrumentService;
    }

    // GET: api/instruments
    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<InstrumentDto>>> GetAllInstruments(
        [FromQuery] PaginationParams pagination)
    {
        var instruments = await _instrumentService.GetInstrumentsPageAsync(pagination);
        return GenerateActionResult(instruments);
    }

    // GET api/instruments/1
    [HttpGet("{id}")]
    public async Task<ActionResult<InstrumentDto>> GetInstrument(int id)
    {
        var instrument = await _instrumentService.GetInstrumentAsync(id);
        return GenerateActionResult(instrument);
    }

    // GET api/instruments/1/profit?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z
    [HttpGet("{id}/profit")]
    public async Task<ActionResult<EntityProfitDto>> GetInstrumentProfit(int id,
        [FromQuery] DateRangeParams dateRange)
    {
        var profit = await _instrumentService.GetInstrumentProfitAsync(id, dateRange);
        return GenerateActionResult(profit);
    }

    // GET api/instruments/1/performance?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z
    [HttpGet("{id}/performance")]
    public async Task<ActionResult<EntityPerformanceDto>> GetInstrumentPerformance(int id,
        [FromQuery] DateRangeParams dateRange)
    {
        var performance = await _instrumentService.GetInstrumentPerformanceAsync(id, dateRange);
        return GenerateActionResult(performance);
    }

    // GET api/instruments/1/prices/chart?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z&frequency=day
    [HttpGet("{id}/prices/chart")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetInstrumentChartedPrices(int id,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency,
        [FromQuery] string currency = null)
    {
        var result = await _instrumentService.ChartInstrumentPricesAsync(id, dateRange, frequency, currency);
        return GenerateActionResult(result);
    }

    // GET api/instruments/1/profit/chart?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z&frequency=day
    [HttpGet("{id}/profit/chart")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetInstrumentChartedProfit(int id,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency,
        [FromQuery] string currency = null)
    {
        var result = await _instrumentService.ChartInstrumentProfitAsync(id, dateRange, frequency, currency);
        return GenerateActionResult(result);
    }

    // GET api/instruments/1/performance/chart?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z&frequency=day
    [HttpGet("{id}/performance/chart")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetInstrumentChartedPerformance(int id,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency)
    {
        var result = await _instrumentService.ChartInstrumentPerformanceAsync(id, dateRange, frequency);
        return GenerateActionResult(result);
    }

    // GET api/instruments/1/profit/chart/aggregated?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z&frequency=day
    [HttpGet("{id}/profit/chart/aggregated")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetInstrumentAggregatedProfit(int id,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency,
        [FromQuery] string currency = null)
    {
        var result = await _instrumentService.ChartInstrumentAggregatedProfitAsync(id, dateRange, frequency, currency);
        return GenerateActionResult(result);
    }

    // GET api/instruments/1/performance/chart/aggregated?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z&frequency=day
    [HttpGet("{id}/performance/chart/aggregated")]
    public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetInstrumentAggregatedPerformance(int id,
        [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency)
    {
        var result = await _instrumentService.ChartInstrumentAggregatedPerformanceAsync(id, dateRange, frequency);
        return GenerateActionResult(result);
    }

    // POST api/instruments
    [HttpPost]
    public async Task<ActionResult<InstrumentDto>> PostInstrument([FromBody] InstrumentDto createRequest)
    {
        var instrument = await _instrumentService.CreateInstrumentAsync(createRequest);

        return GenerateActionResult(instrument, nameof(GetInstrument), new { id = instrument.Response.Id });
    }

    // PUT api/instruments/5
    [HttpPut("{id}")]
    public async Task<ActionResult<InstrumentDto>> PutInstrument(int id, [FromBody] InstrumentDto updateRequest)
    {
        if (id != updateRequest.Id)
        {
            return BadRequest("URL id and request body id don't match.");
        }

        var updatedInstrument = await _instrumentService.UpdateInstrumentAsync(updateRequest);
        return GenerateActionResult(updatedInstrument);
    }

    // DELETE api/instruments/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInstrument(int id)
    {
        var response = await _instrumentService.DeleteAsync(id);
        return GenerateActionResult(response);
    }
}