using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Application.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Services.Queries;

namespace PortEval.Application.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly ICurrencyQueries _currencyQueries;
        private readonly ILogger _logger;

        public CurrenciesController(ICurrencyService currencyService, ICurrencyQueries currencyQueries, ILoggerFactory loggerFactory)
        {
            _currencyService = currencyService;
            _currencyQueries = currencyQueries;
            _logger = loggerFactory.CreateLogger(typeof(CurrenciesController));
        }

        // GET: api/currencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetAllCurrencies()
        {
            _logger.LogInformation("All currencies requested.");

            var currencies = await _currencyQueries.GetAllCurrencies();
            return currencies.Response.ToList();
        }

        // GET api/currencies/USD
        [HttpGet("{code}")]
        public async Task<ActionResult<CurrencyDto>> GetCurrency(string code)
        {
            _logger.LogInformation($"Currency {code} requested.");

            var currency = await _currencyQueries.GetCurrency(code);
            if (currency.Status == QueryStatus.NotFound)
            {
                return NotFound($"Currency {code} not found.");
            }
            return currency.Response;
        }

        // PUT api/currencies/USD
        [HttpPut("{code}")]
        public async Task<IActionResult> UpdateCurrency(string code, [FromBody] CurrencyDto options)
        {
            _logger.LogInformation($"Updating currency {options.Code}.");

            if (code != options.Code)
            {
                return BadRequest($"URL currency code and body request currency code don't match.");
            }

            await _currencyService.UpdateAsync(options);
            return Ok();
        }
    }
}
