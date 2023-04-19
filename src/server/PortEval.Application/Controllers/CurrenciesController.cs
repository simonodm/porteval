using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Controllers;

[Route("currencies")]
[ApiController]
public class CurrenciesController : PortEvalControllerBase
{
    private readonly ICurrencyService _currencyService;

    public CurrenciesController(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    // GET: api/currencies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CurrencyDto>>> GetAllCurrencies()
    {
        var currencies = await _currencyService.GetAllCurrenciesAsync();
        return GenerateActionResult(currencies);
    }

    // GET api/currencies/USD
    [HttpGet("{code}")]
    public async Task<ActionResult<CurrencyDto>> GetCurrency(string code)
    {
        var currency = await _currencyService.GetCurrencyAsync(code);
        return GenerateActionResult(currency);
    }

    // PUT api/currencies/USD
    [HttpPut("{code}")]
    public async Task<ActionResult<CurrencyDto>> UpdateCurrency(string code, [FromBody] CurrencyDto options)
    {
        if (code != options.Code) return BadRequest("URL currency code and body request currency code don't match.");

        var updatedCurrency = await _currencyService.UpdateAsync(options);
        return GenerateActionResult(updatedCurrency);
    }
}