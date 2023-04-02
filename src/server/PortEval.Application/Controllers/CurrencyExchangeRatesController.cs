using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Queries;

namespace PortEval.Application.Controllers
{
    [Route("currencies/{codeFrom}/exchange_rates")]
    [ApiController]
    public class CurrencyExchangeRatesController : ControllerBase
    {
        private readonly ICurrencyExchangeRateQueries _exchangeRateQueries;

        public CurrencyExchangeRatesController(ICurrencyExchangeRateQueries exchangeRateQueries)
        {
            _exchangeRateQueries = exchangeRateQueries;
        }

        // GET: api/currencies/USD/exchange_rates?time=2021-01-01T12:00:00Z
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRates(string codeFrom, [FromQuery] DateTime? time)
        {
            var exchangeRates = await _exchangeRateQueries.GetExchangeRates(codeFrom, time ?? DateTime.UtcNow);
            if (exchangeRates.Status == QueryStatus.NotFound)
            {
                return NotFound($"Currency {codeFrom} not found.");
            }
            return exchangeRates.Response.ToList();
        }

        // GET: api/currencies/USD/exchange_rates/latest
        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<CurrencyExchangeRateDto>>> GetLatestExchangeRates(string codeFrom)
        {
            var exchangeRates = await _exchangeRateQueries.GetExchangeRates(codeFrom, DateTime.UtcNow);
            if (exchangeRates.Status == QueryStatus.NotFound)
            {
                return NotFound($"Currency {codeFrom} not found.");
            }

            return exchangeRates.Response.ToList();
        }

        // GET api/currencies/USD/exchange_rates/EUR/latest
        [HttpGet("{codeTo}/latest")]
        public async Task<ActionResult<CurrencyExchangeRateDto>> GetLatestExchangeRate(string codeFrom, string codeTo)
        {
            var exchangeRate = await _exchangeRateQueries.GetExchangeRateAt(codeFrom, codeTo, DateTime.UtcNow);
            if (exchangeRate.Status == QueryStatus.NotFound)
            {
                return NotFound($"Invalid currency pair: {codeFrom}, {codeTo}.");
            }
            return exchangeRate.Response;
        }

        // GET api/currencies/USD/exchange_rates/EUR/at?time=2021-01-01T00:00:00
        [HttpGet("{codeTo}/at")]
        public async Task<ActionResult<CurrencyExchangeRateDto>> GetExchangeRateAt(string codeFrom, string codeTo, [FromQuery] DateTime time)
        {
            var exchangeRate = await _exchangeRateQueries.GetExchangeRateAt(codeFrom, codeTo, time);
            if (exchangeRate.Status == QueryStatus.NotFound)
            {
                return NotFound($"Invalid currency pair: {codeFrom}, {codeTo}.");
            }

            return exchangeRate.Response;
        }
    }
}
