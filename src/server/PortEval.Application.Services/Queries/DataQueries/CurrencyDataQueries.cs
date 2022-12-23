using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using PortEval.Application.Features.Queries.Models;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Features.Queries.DataQueries
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

        public static QueryWrapper<CurrencyDto> GetDefaultCurrency()
        {
            return new QueryWrapper<CurrencyDto>
            {
                Query = @"SELECT Code, Name, Symbol, IsDefault FROM dbo.Currencies
                          WHERE IsDefault = 1"
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

        public static QueryWrapper<IEnumerable<CurrencyExchangeRateDto>> GetDirectExchangeRates(string baseCurrencyCode,
            string targetCurrencyCode, DateRangeParams dateRange)
        {
            return new QueryWrapper<IEnumerable<CurrencyExchangeRateDto>>
            {
                Query = @$"SELECT * FROM dbo.CurrencyExchangeRates
                          WHERE CurrencyFromCode = @BaseCurrencyCode
                          AND CurrencyToCode = @TargetCurrencyCode
                          AND Time >= @TimeFrom
                          AND Time <= @TimeTo
                          ORDER BY Time",
                Params = new
                {
                    BaseCurrencyCode = baseCurrencyCode, TargetCurrencyCode = targetCurrencyCode,
                    TimeFrom = dateRange.From, TimeTo = dateRange.To
                }
            };
        }

        public static QueryWrapper<IEnumerable<CurrencyExchangeRateDto>> GetInverselyCalculatedExchangeRates(string baseCurrencyCode,
            string targetCurrencyCode, DateRangeParams dateRange)
        {
            return new QueryWrapper<IEnumerable<CurrencyExchangeRateDto>>
            {
                Query = @"SELECT CurrencyToCode AS CurrencyFromCode, CurrencyFromCode AS CurrencyToCode, Time, 1/ExchangeRate AS ExchangeRate FROM dbo.CurrencyExchangeRates
                          WHERE CurrencyFromCode = @TargetCurrencyCode
                          AND CurrencyToCode = @BaseCurrencyCode
                          AND Time >= @TimeFrom
                          AND Time <= @TimeTo
                          ORDER BY Time",
                Params = new
                {
                    BaseCurrencyCode = baseCurrencyCode,
                    TargetCurrencyCode = targetCurrencyCode,
                    TimeFrom = dateRange.From,
                    TimeTo = dateRange.To
                }
            };
        }
    }
}
