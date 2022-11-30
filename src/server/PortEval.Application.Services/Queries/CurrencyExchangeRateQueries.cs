using Dapper;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Queries.DataQueries;
using PortEval.Application.Services.Queries.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Queries
{
    /// <inheritdoc cref="ICurrencyExchangeRateQueries"/>
    public class CurrencyExchangeRateQueries : ICurrencyExchangeRateQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;
        private readonly ICurrencyQueries _currencyQueries;

        public CurrencyExchangeRateQueries(IDbConnectionCreator connection, ICurrencyQueries currencyQueries)
        {
            _connectionCreator = connection;
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

            using (var connection = _connectionCreator.CreateConnection())
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

            using var connection = _connectionCreator.CreateConnection();
            var rate = await connection.QueryFirstOrDefaultAsync<CurrencyExchangeRateDto>(query.Query, query.Params);

            return new QueryResponse<CurrencyExchangeRateDto>
            {
                Status = rate == default ? QueryStatus.NotFound : QueryStatus.Ok,
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

        /// <inheritdoc cref="ICurrencyExchangeRateQueries.Convert"/>
        public async Task<decimal> Convert(string baseCurrencyCode, string targetCurrencyCode, decimal price, DateTime time)
        {
            if (baseCurrencyCode == targetCurrencyCode || price == 0)
            {
                return price;
            }

            var defaultCurrencyQuery = CurrencyDataQueries.GetDefaultCurrency();
            var exchangeRateQuery = CurrencyDataQueries.GetCurrencyExchangeRate(baseCurrencyCode, targetCurrencyCode, time);

            using var connection = _connectionCreator.CreateConnection();
            var defaultCurrency =
                await connection.QueryFirstOrDefaultAsync<CurrencyDto>(defaultCurrencyQuery.Query,
                    defaultCurrencyQuery.Params);

            var exchangeRate = await connection.QueryFirstOrDefaultAsync<CurrencyExchangeRateDto>(exchangeRateQuery.Query, exchangeRateQuery.Params);
            
            if(exchangeRate != null)
            {
                return price * exchangeRate.ExchangeRate;
            }
            if (targetCurrencyCode == defaultCurrency.Code)
            {
                return price * price / await Convert(defaultCurrency.Code, baseCurrencyCode, price, time);
            }
            if (baseCurrencyCode != defaultCurrency.Code)
            {
                var convertedToDefault = await Convert(baseCurrencyCode, defaultCurrency.Code, price, time);
                return await Convert(defaultCurrency.Code, targetCurrencyCode, convertedToDefault, time);
            }

            throw new Exception($"No exchange rate available from {baseCurrencyCode} to {targetCurrencyCode} at {time}.");
        }
    }
}
