using Dapper;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Queries.DataQueries;
using PortEval.Application.Queries.Interfaces;
using PortEval.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Queries
{
    /// <inheritdoc cref="ICurrencyExchangeRateQueries"/>
    public class CurrencyExchangeRateQueries : ICurrencyExchangeRateQueries
    {
        private readonly PortEvalDbConnection _connection;

        public CurrencyExchangeRateQueries(PortEvalDbConnection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc cref="ICurrencyExchangeRateQueries.GetExchangeRates"/>
        public async Task<QueryResponse<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRates(string currencyCode, DateTime time)
        {
            var currencyQuery = CurrencyDataQueries.GetCurrencyQuery(currencyCode);
            var exchangeableCurrenciesQuery = CurrencyDataQueries.GetExchangeableCurrencies(currencyCode, time);
            var exchangeRates = new List<CurrencyExchangeRateDto>();

            IEnumerable<CurrencyDto> exchangeableCurrencies;

            using (var connection = _connection.CreateConnection())
            {
                var currency = await connection.QueryFirstOrDefaultAsync<CurrencyDto>(currencyQuery.Query, currencyQuery.Params);
                if(currency == null)
                {
                    return new QueryResponse<IEnumerable<CurrencyExchangeRateDto>>
                    {
                        Status = QueryStatus.NotFound
                    };
                }

                exchangeableCurrencies =
                    await connection.QueryAsync<CurrencyDto>(exchangeableCurrenciesQuery.Query,
                        exchangeableCurrenciesQuery.Params);
            }

            foreach (var currency in exchangeableCurrencies)
            {
                var exchangeRate = await GetExchangeRateAt(currencyCode, currency.Code, time);
                if(exchangeRate.Status == QueryStatus.Ok)
                {
                    exchangeRates.Add(exchangeRate.Response);
                }
            }

            return new QueryResponse<IEnumerable<CurrencyExchangeRateDto>>
            {
                Status = QueryStatus.Ok,
                Response = exchangeRates
            };
        }

        /// <inheritdoc cref="ICurrencyExchangeRateQueries.GetExchangeRateAt"/>
        public async Task<QueryResponse<CurrencyExchangeRateDto>> GetExchangeRateAt(string baseCurrencyCode, string targetCurrencyCode, DateTime time)
        {
            var query = CurrencyDataQueries.GetCurrencyExchangeRate(baseCurrencyCode, targetCurrencyCode, time);

            using var connection = _connection.CreateConnection();
            var rate = await connection.QueryFirstOrDefaultAsync<CurrencyExchangeRateDto>(query.Query, query.Params);

            return new QueryResponse<CurrencyExchangeRateDto>
            {
                Status = rate != null ? QueryStatus.Ok : QueryStatus.NotFound,
                Response = rate
            };
        }

        /// <inheritdoc cref="ICurrencyExchangeRateQueries.ConvertChartPointCurrency"/>
        public async Task<EntityChartPointDto> ConvertChartPointCurrency(string baseCurrencyCode,
            string targetCurrencyCode, EntityChartPointDto chartPoint)
        {
            if (targetCurrencyCode == null || baseCurrencyCode == targetCurrencyCode) return chartPoint;

            var convertedPrice = await Convert(baseCurrencyCode, targetCurrencyCode,
                chartPoint.Value, chartPoint.Time);

            return chartPoint.ChangeValue(convertedPrice);
        }

        private async Task<decimal> Convert(string baseCurrencyCode, string targetCurrencyCode, decimal price, DateTime time)
        {
            var query = CurrencyDataQueries.GetCurrencyExchangeRate(baseCurrencyCode, targetCurrencyCode, time);

            using var connection = _connection.CreateConnection();
            var exchangeRate =
                await connection.QueryFirstOrDefaultAsync<CurrencyExchangeRateDto>(query.Query, query.Params);

            if (exchangeRate == null)
            {
                throw new Exception($"No exchange rate available from {baseCurrencyCode} to {targetCurrencyCode} at {time}.");
            }

            return price * exchangeRate.ExchangeRate;
        }
    }
}
