using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Queries.DataQueries
{
    internal static class CurrencyDataQueries
    {
        public static QueryWrapper<IEnumerable<CurrencyDto>> GetAllCurrenciesQuery()
        {
            return new QueryWrapper<IEnumerable<CurrencyDto>>
            {
                Query = "SELECT Code, Name, Symbol, IsDefault FROM dbo.Currencies"
            };
        }

        public static QueryWrapper<CurrencyDto> GetCurrencyQuery(string currencyCode)
        {
            return new QueryWrapper<CurrencyDto>
            {
                Query = @"SELECT Code, Name, Symbol, IsDefault FROM dbo.Currencies
                          WHERE Code = @CurrencyCode",
                Params = new { CurrencyCode = currencyCode }
            };
        }

        public static QueryWrapper<CurrencyExchangeRateDto> GetCurrencyExchangeRate(string currencyCode, string currencyToCode, DateTime time)
        {
            return new QueryWrapper<CurrencyExchangeRateDto>
            {
                Query = @"SELECT TOP 1 * FROM dbo.CurrencyExchangeRates
                         WHERE CurrencyFromCode = @BaseCurrencyCode
                         AND CurrencyToCode = @TargetCurrencyCode
                         AND Time <= @Time
                         ORDER BY Time DESC",
                Params = new { BaseCurrencyCode = currencyCode, TargetCurrencyCode = currencyToCode, Time = time }
            };
        }

        public static QueryWrapper<IEnumerable<CurrencyDto>> GetExchangeableCurrencies(string baseCurrencyCode, DateTime time)
        {
            return new QueryWrapper<IEnumerable<CurrencyDto>>
            {
                Query = @"SELECT DISTINCT Code, Name, Symbol, IsDefault FROM dbo.Currencies
                          INNER JOIN (SELECT * FROM dbo.CurrencyExchangeRates WHERE Time <= @Time AND CurrencyFromCode = @CurrencyCode) as ER
                          ON ER.CurrencyToCode = Currencies.Code
                          ORDER BY Code",
                Params = new { CurrencyCode = baseCurrencyCode, Time = time }
            };
        }
    }
}
