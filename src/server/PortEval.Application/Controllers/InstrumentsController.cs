using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Application.Services.Interfaces;
using PortEval.Domain.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Services.Queries;

namespace PortEval.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstrumentsController : ControllerBase
    {
        private readonly IInstrumentService _instrumentService;
        private readonly IInstrumentQueries _instrumentQueries;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public InstrumentsController(IInstrumentService instrumentService, IInstrumentQueries instrumentQueries, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _instrumentService = instrumentService;
            _instrumentQueries = instrumentQueries;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger(typeof(InstrumentsController));
        }

        // GET: api/instruments
        [HttpGet]
        public async Task<PaginatedResponse<InstrumentDto>> GetAllInstruments([FromQuery] PaginationParams pagination)
        {
            _logger.LogInformation($"Instruments requested (page {pagination.Page}, limit {pagination.Limit}.");

            var instruments = await _instrumentQueries.GetInstrumentsPage(pagination);
            return instruments.Response;
        }

        // GET api/instruments/1
        [HttpGet("{id}")]
        public async Task<ActionResult<InstrumentDto>> GetInstrument(int id)
        {
            _logger.LogInformation($"Instrument {id} requested.");

            var instrument = await _instrumentQueries.GetInstrument(id);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {id} not found.");
            }

            return instrument.Response;
        }

        // GET api/instruments/1/profit?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z
        [HttpGet("{id}/profit")]
        public async Task<ActionResult<EntityProfitDto>> GetInstrumentProfit(int id,
            [FromQuery] DateRangeParams dateRange)
        {
            _logger.LogInformation($"Instrument {id} profit between {dateRange.From} and {dateRange.To} requested.");

            var profit = await _instrumentQueries.GetInstrumentProfit(id, dateRange);
            if (profit.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {id} not found.");
            }

            return profit.Response;
        }

        // GET api/instruments/1/performance?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z
        [HttpGet("{id}/performance")]
        public async Task<ActionResult<EntityPerformanceDto>> GetInstrumentPerformance(int id,
            [FromQuery] DateRangeParams dateRange)
        {
            _logger.LogInformation($"Instrument {id} performance between {dateRange.From} and {dateRange.To} requested.");

            var performance  = await _instrumentQueries.GetInstrumentPerformance(id, dateRange);
            if (performance.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {id} not found.");
            }

            return performance.Response;
        }

        // GET api/instruments/1/prices/chart?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z&frequency=day
        [HttpGet("{id}/prices/chart")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetInstrumentChartedPrices(int id,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency, [FromQuery] string currency = null)
        {
            _logger.LogInformation($"Instrument {id} price chart between {dateRange.From} and {dateRange.To} with {frequency} interval requested.");

            var result = await _instrumentQueries.ChartInstrumentPrices(id, dateRange, frequency, currency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {id} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/instruments/1/profit/chart?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z&frequency=day
        [HttpGet("{id}/profit/chart")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetInstrumentChartedProfit(int id,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency, [FromQuery] string currency = null)
        {
            _logger.LogInformation($"Instrument {id} profit chart between {dateRange.From} and {dateRange.To} with {frequency} interval requested.");

            var result = await _instrumentQueries.ChartInstrumentProfit(id, dateRange, frequency, currency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {id} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/instruments/1/performance/chart?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z&frequency=day
        [HttpGet("{id}/performance/chart")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetInstrumentChartedPerformance(int id,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency)
        {
            _logger.LogInformation($"Instrument {id} performance chart between {dateRange.From} and {dateRange.To} with {frequency} interval requested.");

            var result = await _instrumentQueries.ChartInstrumentPerformance(id, dateRange, frequency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {id} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/instruments/1/profit/chart/aggregated?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z&frequency=day
        [HttpGet("{id}/profit/chart/aggregated")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetInstrumentAggregatedProfit(int id,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency, [FromQuery] string currency = null)
        {
            _logger.LogInformation($"Instrument {id} aggregated profit chart between {dateRange.From} and {dateRange.To} with {frequency} interval requested.");

            var result = await _instrumentQueries.ChartInstrumentProfitAggregated(id, dateRange, frequency, currency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {id} not found.");
            }

            return result.Response.ToList();
        }

        // GET api/instruments/1/performance/chart/aggregated?from=2021-01-01T00:00:00Z&to=2021-01-08T00:00:00Z&frequency=day
        [HttpGet("{id}/performance/chart/aggregated")]
        public async Task<ActionResult<IEnumerable<EntityChartPointDto>>> GetInstrumentAggregatedPerformance(int id,
            [FromQuery] DateRangeParams dateRange, [FromQuery] AggregationFrequency frequency)
        {
            _logger.LogInformation($"Instrument {id} aggregated performance chart between {dateRange.From} and {dateRange.To} with {frequency} interval requested.");

            var result = await _instrumentQueries.ChartInstrumentPerformanceAggregated(id, dateRange, frequency);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {id} not found.");
            }

            return result.Response.ToList();
        }

        // POST api/instruments
        [HttpPost]
        public async Task<ActionResult<InstrumentDto>> PostInstrument([FromBody] InstrumentDto createRequest)
        {
            _logger.LogInformation($"Creating instrument {createRequest.Name}.");

            var instrument = await _instrumentService.CreateInstrumentAsync(createRequest);

            return CreatedAtAction("GetInstrument", new { id = instrument.Id },
                _mapper.Map<InstrumentDto>(instrument));
        }

        // PUT api/instruments/5
        [HttpPut("{id}")]
        public async Task<ActionResult<InstrumentDto>> PutInstrument(int id, [FromBody] InstrumentDto updateRequest)
        {
            _logger.LogInformation($"Updating instrument {updateRequest.Id}.");

            if (id != updateRequest.Id)
            {
                return BadRequest("URL id and request body id don't match.");
            }

            var updatedInstrument = await _instrumentService.UpdateInstrumentAsync(updateRequest);
            return _mapper.Map<InstrumentDto>(updatedInstrument);
        }

        // DELETE api/instruments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstrument(int id)
        {
            _logger.LogInformation($"Deleting instrument {id}.");

            await _instrumentService.DeleteAsync(id);
            return Ok();
        }
    }
}
