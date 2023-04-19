using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Controllers;

[Route("currencies/{codeFrom}/exchange_rates")]
[ApiController]
public class CurrencyExchangeRatesController : PortEvalControllerBase
{
    private readonly ICurrencyExchangeRateService _currencyExchangeRateService;

    public CurrencyExchangeRatesController(ICurrencyExchangeRateService exchangeRateService)
    {
        _currencyExchangeRateService = exchangeRateService;
    }

    // GET: api/currencies/USD/exchange_rates?time=2021-01-01T12:00:00Z
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRates(string codeFrom,
        [FromQuery] DateTime? time)
    {
        var exchangeRates = await _currencyExchangeRateService.GetExchangeRatesAsync(codeFrom, time ?? DateTime.UtcNow);
        return GenerateActionResult(exchangeRates);
    }

    // GET: api/currencies/USD/exchange_rates/latest
    [HttpGet("latest")]
    public async Task<ActionResult<IEnumerable<CurrencyExchangeRateDto>>> GetLatestExchangeRates(string codeFrom)
    {
        var exchangeRates = await _currencyExchangeRateService.GetExchangeRatesAsync(codeFrom, DateTime.UtcNow);
        return GenerateActionResult(exchangeRates);
    }

    // GET api/currencies/USD/exchange_rates/EUR/latest
    [HttpGet("{codeTo}/latest")]
    public async Task<ActionResult<CurrencyExchangeRateDto>> GetLatestExchangeRate(string codeFrom, string codeTo)
    {
        var exchangeRate = await _currencyExchangeRateService.GetExchangeRateAtAsync(codeFrom, codeTo, DateTime.UtcNow);
        return GenerateActionResult(exchangeRate);
    }

    // GET api/currencies/USD/exchange_rates/EUR/at?time=2021-01-01T00:00:00
    [HttpGet("{codeTo}/at")]
    public async Task<ActionResult<CurrencyExchangeRateDto>> GetExchangeRateAt(string codeFrom, string codeTo,
        [FromQuery] DateTime time)
    {
        var exchangeRate = await _currencyExchangeRateService.GetExchangeRateAtAsync(codeFrom, codeTo, time);
        return GenerateActionResult(exchangeRate);
    }
}