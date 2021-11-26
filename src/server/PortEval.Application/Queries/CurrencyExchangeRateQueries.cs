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
        private readonly ICurrencyQueries _currencyQueries;

        public CurrencyExchangeRateQueries(PortEvalDbConnection connection, ICurrencyQueries currencyQueries)
        {
            _connection = connection;
            _currencyQueries = currencyQueries;
        }

        /// <inheritdoc cref="ICurrencyExchangeRateQueries.GetExchangeRates"/>
        public async Task<QueryResponse<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRates(string currencyCode, DateTime time)
        {
            var currency = await _currencyQueries.GetCurrency(currencyCode);
            if (currency == null)
            {
                return new QueryResponse<IEnumerable<CurrencyExchangeRateDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }
            var exchangeableCurrenciesQuery = CurrencyDataQueries.GetExchangeableCurrencies(currencyCode, time);
            var exchangeRates = new List<CurrencyExchangeRateDto>();

            IEnumerable<CurrencyDto> exchangeableCurrencies;

            using (var connection = _connection.CreateConnection())
            {
                exchangeableCurrencies =
                    await connection.QueryAsync<CurrencyDto>(exchangeableCurrenciesQuery.Query,
                        exchangeableCurrenciesQuery.Params);
            }

            foreach (var exchangeableCurrency in exchangeableCurrencies)
            {
                var exchangeRate = await GetExchangeRateAt(currencyCode, exchangeableCurrency.Code, time);
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
            var baseCurrency = await _currencyQueries.GetCurrency(baseCurrencyCode);
            var targetCurrency = await _currencyQueries.GetCurrency(targetCurrencyCode);
            if (baseCurrency == null || targetCurrency == null)
            {
                return new QueryResponse<CurrencyExchangeRateDto>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var query = CurrencyDataQueries.GetCurrencyExchangeRate(baseCurrencyCode, targetCurrencyCode, time);

            using var connection = _connection.CreateConnection();
            var rate = await connection.QueryFirstOrDefaultAsync<CurrencyExchangeRateDto>(query.Query, query.Params);

            return new QueryResponse<CurrencyExchangeRateDto>
            {
                Status = QueryStatus.Ok,
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
