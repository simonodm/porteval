using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Queries.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Services.Queries;

namespace PortEval.Application.Controllers
{
    [Route("currencies/{codeFrom}/exchange_rates")]
    [ApiController]
    public class CurrencyExchangeRatesController : ControllerBase
    {
        private readonly ICurrencyExchangeRateQueries _exchangeRateQueries;
        private readonly ILogger _logger;

        public CurrencyExchangeRatesController(ICurrencyExchangeRateQueries exchangeRateQueries, ILoggerFactory loggerFactory)
        {
            _exchangeRateQueries = exchangeRateQueries;
            _logger = loggerFactory.CreateLogger(typeof(CurrencyExchangeRatesController));
        }

        // GET: api/currencies/USD/exchange_rates?time=2021-01-01T12:00:00Z
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRates(string codeFrom, [FromQuery] DateTime? time)
        {
            _logger.LogInformation($"Requesting {codeFrom} exchange rates at {time}.");

            var exchangeRates = await _exchangeRateQueries.GetExchangeRates(codeFrom, time ?? DateTime.Now);
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
            _logger.LogInformation($"Requesting latest {codeFrom} exchange rates.");

            var exchangeRates = await _exchangeRateQueries.GetExchangeRates(codeFrom, DateTime.Now);
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
            _logger.LogInformation($"Latest exchange rate from {codeFrom} to {codeTo} requested.");

            var exchangeRate = await _exchangeRateQueries.GetExchangeRateAt(codeFrom, codeTo, DateTime.Now);
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
            _logger.LogInformation($"Exchange rate between {codeFrom} to {codeTo} at {time} requested.");

            var exchangeRate = await _exchangeRateQueries.GetExchangeRateAt(codeFrom, codeTo, time);
            if (exchangeRate.Status == QueryStatus.NotFound)
            {
                return NotFound($"Invalid currency pair: {codeFrom}, {codeTo}.");
            }

            return exchangeRate.Response;
        }
    }
}
