using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("instruments/{instrumentId}/prices")]
    [ApiController]
    public class InstrumentPricesController : PortEvalControllerBase
    {
        private readonly IInstrumentPriceService _priceService;

        public InstrumentPricesController(IInstrumentPriceService priceService)
        {
            _priceService = priceService;
        }

        // GET: api/instruments/1/prices
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<InstrumentPriceDto>>> GetInstrumentPrices(int instrumentId,
            [FromQuery] DateRangeParams dateRange, [FromQuery] PaginationParams pagination, [FromQuery] AggregationFrequency? frequency = null,
            [FromQuery] bool compressed = false)
        {
            var prices = await _priceService.GetInstrumentPricesPageAsync(instrumentId, pagination, dateRange, compressed, frequency);
            return GenerateActionResult(prices);
        }

        // GET: api/instruments/1/prices/at?time=2021-01-01T00:00:00
        [HttpGet("at")]
        public async Task<ActionResult<InstrumentPriceDto>> GetInstrumentPriceAt(int instrumentId, [FromQuery] DateTime time)
        {
            var price = await _priceService.GetInstrumentPriceAsync(instrumentId, time);
            return GenerateActionResult(price);
        }

        // GET: api/instruments/1/prices/latest
        [HttpGet("latest")]
        public async Task<ActionResult<InstrumentPriceDto>> GetLatestInstrumentPrice(int instrumentId)
        {
            var price = await _priceService.GetInstrumentPriceAsync(instrumentId, DateTime.UtcNow);
            return GenerateActionResult(price);
        }

        // POST api/instruments/1/prices
        [HttpPost]
        public async Task<ActionResult<InstrumentPriceDto>> PostPricePoint(int instrumentId, [FromBody] InstrumentPriceDto createRequest)
        {
            if (instrumentId != createRequest.InstrumentId)
            {
                return BadRequest("URL instrument id and request body instrument id don't match.");
            }

            var price = await _priceService.AddPricePointAsync(createRequest);
            return GenerateActionResult(price, nameof(GetInstrumentPriceAt),
                new { instrumentId = price.Response.InstrumentId, time = price.Response.Time });
        }

        // DELETE api/instruments/1/prices/5
        [HttpDelete("{priceId}")]
        public async Task<IActionResult> DeletePricePoint(int instrumentId, int priceId)
        {
            var response = await _priceService.DeletePricePointByIdAsync(instrumentId, priceId);
            return GenerateActionResult(response);
        }

    }
}
