using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Core.Queries;

namespace PortEval.Application.Controllers
{
    [Route("instruments/{instrumentId}/prices")]
    [ApiController]
    public class InstrumentPricesController : ControllerBase
    {
        private readonly IInstrumentPriceService _priceService;
        private readonly IInstrumentQueries _instrumentQueries;

        public InstrumentPricesController(IInstrumentPriceService priceService, IInstrumentQueries instrumentQueries)
        {
            _priceService = priceService;
            _instrumentQueries = instrumentQueries;
        }

        // GET: api/instruments/1/prices
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<InstrumentPriceDto>>> GetInstrumentPrices(int instrumentId,
            [FromQuery] DateRangeParams dateRange, [FromQuery] PaginationParams pagination, [FromQuery] AggregationFrequency? frequency = null,
            [FromQuery] bool compressed = false)
        {
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
            var price = await _instrumentQueries.GetInstrumentPrice(instrumentId, DateTime.UtcNow);
            if (price.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {instrumentId} not found.");
            }

            return price.Response;
        }

        // POST api/instruments/1/prices
        [HttpPost]
        public async Task<IActionResult> PostPricePoint(int instrumentId, [FromBody] InstrumentPriceDto createRequest)
        {
            if (instrumentId != createRequest.InstrumentId)
            {
                return BadRequest("URL instrument id and request body instrument id don't match.");
            }

            var price = await _priceService.AddPricePointAsync(createRequest);
            return CreatedAtAction(nameof(GetInstrumentPriceAt), new { instrumentId = price.Id, time = price.Time }, null);
        }

        // DELETE api/instruments/1/prices/5
        [HttpDelete("{priceId}")]
        public async Task<IActionResult> DeletePricePoint(int instrumentId, int priceId)
        {
            await _priceService.DeletePricePointByIdAsync(instrumentId, priceId);
            return Ok();
        }

    }
}
