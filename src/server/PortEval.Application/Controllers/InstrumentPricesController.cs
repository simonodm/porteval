using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Application.Services.Interfaces;
using System;
using System.Threading.Tasks;
using PortEval.Application.Services.Queries;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Controllers
{
    [Route("instruments/{instrumentId}/prices")]
    [ApiController]
    public class InstrumentPricesController : ControllerBase
    {
        private readonly IInstrumentPriceService _priceService;
        private readonly IInstrumentQueries _instrumentQueries;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public InstrumentPricesController(IInstrumentPriceService priceService, IInstrumentQueries instrumentQueries, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _priceService = priceService;
            _instrumentQueries = instrumentQueries;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger(typeof(InstrumentPricesController));
        }

        // GET: api/instruments/1/prices
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<InstrumentPriceDto>>> GetInstrumentPrices(int instrumentId,
            [FromQuery] DateRangeParams dateRange, [FromQuery] PaginationParams pagination, [FromQuery] AggregationFrequency? frequency = null,
            [FromQuery] bool compressed = false)
        {
            _logger.LogInformation($"Prices requested for instrument {instrumentId} (page {pagination.Page}, limit {pagination.Limit}).");

            var prices = await _instrumentQueries.GetInstrumentPricesPage(instrumentId, pagination, dateRange, compressed, frequency);
            if (prices.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {instrumentId} not found.");
            }

            return prices.Response;
        }

        // GET: api/instruments/1/prices/at?time=2021-01-01T00:00:00
        [HttpGet("at")]
        public async Task<ActionResult<InstrumentPriceDto>> GetInstrumentPriceAt(int instrumentId, [FromQuery] DateTime time)
        {
            _logger.LogInformation($"Price at {time} requested for instrument {instrumentId}.");

            var price = await _instrumentQueries.GetInstrumentPrice(instrumentId, time);
            if (price.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {instrumentId} not found.");
            }

            return price.Response;
        }

        // GET: api/instruments/1/prices/latest
        [HttpGet("latest")]
        public async Task<ActionResult<InstrumentPriceDto>> GetLatestInstrumentPrice(int instrumentId)
        {
            _logger.LogInformation($"Current price of instrument {instrumentId} requested.");

            var price = await _instrumentQueries.GetInstrumentPrice(instrumentId, DateTime.UtcNow);
            if (price.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {instrumentId} not found.");
            }

            return price.Response;
        }

        // POST api/instruments/1/prices
        [HttpPost]
        public async Task<ActionResult<InstrumentPriceDto>> PostPricePoint(int instrumentId, [FromBody] InstrumentPriceDto createRequest)
        {
            _logger.LogInformation($"Adding price point at {createRequest.Time} for instrument {instrumentId}.");

            if (instrumentId != createRequest.InstrumentId)
            {
                return BadRequest("URL instrument id and request body instrument id don't match.");
            }

            var price = await _priceService.AddPricePointAsync(createRequest);
            return CreatedAtAction("GetInstrumentPriceAt", new { instrumentId = price.Id, time = price.Time }, _mapper.Map<InstrumentPriceDto>(price));
        }

        // DELETE api/instruments/1/prices/5
        [HttpDelete("{priceId}")]
        public async Task<IActionResult> DeletePricePoint(int instrumentId, int priceId)
        {
            _logger.LogInformation($"Deleting price point {priceId} of instrument {instrumentId}.");

            await _priceService.DeletePricePointByIdAsync(instrumentId, priceId);
            return Ok();
        }

    }
}
