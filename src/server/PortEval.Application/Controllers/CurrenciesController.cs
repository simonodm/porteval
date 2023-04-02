using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Core.Queries;

namespace PortEval.Application.Controllers
{
    [Route("currencies")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly ICurrencyQueries _currencyQueries;

        public CurrenciesController(ICurrencyService currencyService, ICurrencyQueries currencyQueries)
        {
            _currencyService = currencyService;
            _currencyQueries = currencyQueries;
        }

        // GET: api/currencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetAllCurrencies()
        {
            var currencies = await _currencyQueries.GetAllCurrencies();
            return currencies.Response.ToList();
        }

        // GET api/currencies/USD
        [HttpGet("{code}")]
        public async Task<ActionResult<CurrencyDto>> GetCurrency(string code)
        {
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
            if (code != options.Code)
            {
                return BadRequest($"URL currency code and body request currency code don't match.");
            }

            await _currencyService.UpdateAsync(options);
            return Ok();
        }
    }
}
