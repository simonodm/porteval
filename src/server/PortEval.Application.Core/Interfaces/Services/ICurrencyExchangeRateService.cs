using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Interfaces.Services;

public interface ICurrencyExchangeRateService
{
    Task<OperationResponse<CurrencyExchangeRateDto>> GetExchangeRateAtAsync(string currencyFrom, string currencyTo, DateTime
        time);

    Task<OperationResponse<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRatesAsync(string currencyCode,
        DateTime time);

    Task<OperationResponse<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRatesAsync(string currencyFrom,
        string currencyTo, DateRangeParams dateRange);
}