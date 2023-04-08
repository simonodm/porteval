using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Interfaces.DataQueries
{
    public interface ICurrencyDataQueries
    {
        Task<IEnumerable<CurrencyDto>> GetAllCurrenciesAsync();
        Task<CurrencyDto> GetCurrencyAsync(string currencyCode);
        Task<CurrencyDto> GetDefaultCurrencyAsync();

        Task<CurrencyExchangeRateDto> GetCurrencyExchangeRateAsync(string currencyFrom, string currencyTo,
            DateTime time);

        Task<IEnumerable<CurrencyDto>> GetExchangeableCurrenciesAsync(string baseCurrencyCode,
            DateTime time);

        Task<IEnumerable<CurrencyExchangeRateDto>> GetDirectExchangeRatesAsync(string baseCurrencyCode,
            string targetCurrencyCode, DateRangeParams dateRange);

        Task<IEnumerable<CurrencyExchangeRateDto>> GetInversedExchangeRatesAsync(string baseCurrencyCode,
            string targetCurrencyCode, DateRangeParams dateRange);
    }
}