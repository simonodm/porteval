using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Queries.DataQueries;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Queries
{
    /// <inheritdoc cref="ICurrencyExchangeRateQueries"/>
    public class CurrencyExchangeRateQueries : ICurrencyExchangeRateQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;
        private readonly ICurrencyQueries _currencyQueries;

        private readonly ICurrencyConverter _currencyConverter;

        public CurrencyExchangeRateQueries(IDbConnectionCreator connection, ICurrencyQueries currencyQueries, ICurrencyConverter currencyConverter)
        {
            _connectionCreator = connection;
            _currencyQueries = currencyQueries;
            _currencyConverter = currencyConverter;
        }

        /// <inheritdoc />
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
                if (exchangeRate.Status == QueryStatus.Ok)
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

        /// <inheritdoc />
        public async Task<QueryResponse<IEnumerable<CurrencyExchangeRateDto>>> GetExchangeRates(string baseCurrencyCode,
            string targetCurrencyCode, DateRangeParams dateRange)
        {
            var baseCurrencyQueryResponse = await _currencyQueries.GetCurrency(baseCurrencyCode);
            if (baseCurrencyQueryResponse.Status != QueryStatus.Ok)
            {
                return new QueryResponse<IEnumerable<CurrencyExchangeRateDto>>
                {
                    Status = baseCurrencyQueryResponse.Status
                };
            }

            var targetCurrencyQueryResponse = await _currencyQueries.GetCurrency(targetCurrencyCode);
            if (targetCurrencyQueryResponse.Status != QueryStatus.Ok)
            {
                return new QueryResponse<IEnumerable<CurrencyExchangeRateDto>>
                {
                    Status = targetCurrencyQueryResponse.Status
                };
            }

            IEnumerable<CurrencyExchangeRateDto> exchangeRates;

            if (baseCurrencyQueryResponse.Response.IsDefault)
            {
                exchangeRates = await GetDirectExchangeRates(baseCurrencyCode, targetCurrencyCode, dateRange);
            }
            else if (targetCurrencyQueryResponse.Response.IsDefault)
            {
                exchangeRates =
                    await GetInverselyCalculatedExchangeRates(baseCurrencyCode, targetCurrencyCode, dateRange);
            }
            else
            {
                exchangeRates =
                    await GetIndirectlyCalculatedExchangeRates(baseCurrencyCode, targetCurrencyCode, dateRange);
            }

            return new QueryResponse<IEnumerable<CurrencyExchangeRateDto>>
            {
                Status = QueryStatus.Ok,
                Response = exchangeRates
            };
        }

        /// <inheritdoc />
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

        private async Task<IEnumerable<CurrencyExchangeRateDto>> GetDirectExchangeRates(string baseCurrencyCode,
            string targetCurrencyCode, DateRangeParams dateRange)
        {
            var query = CurrencyDataQueries.GetDirectExchangeRates(baseCurrencyCode, targetCurrencyCode, dateRange);

            using var connection = _connectionCreator.CreateConnection();
            var exchangeRates = await connection.QueryAsync<CurrencyExchangeRateDto>(query.Query, query.Params);

            return exchangeRates;
        }

        private async Task<IEnumerable<CurrencyExchangeRateDto>> GetInverselyCalculatedExchangeRates(string baseCurrencyCode,
            string targetCurrencyCode, DateRangeParams dateRange)
        {
            var query = CurrencyDataQueries.GetInverselyCalculatedExchangeRates(baseCurrencyCode, targetCurrencyCode, dateRange);

            using var connection = _connectionCreator.CreateConnection();
            var exchangeRates = await connection.QueryAsync<CurrencyExchangeRateDto>(query.Query, query.Params);

            return exchangeRates;
        }

        private async Task<IEnumerable<CurrencyExchangeRateDto>> GetIndirectlyCalculatedExchangeRates(string baseCurrencyCode,
            string targetCurrencyCode, DateRangeParams dateRange)
        {
            var defaultCurrencyQuery = CurrencyDataQueries.GetDefaultCurrency();

            using var connection = _connectionCreator.CreateConnection();

            var defaultCurrency =
                await connection.QueryFirstOrDefaultAsync<CurrencyDto>(defaultCurrencyQuery.Query, defaultCurrencyQuery.Params);

            if (defaultCurrency == null)
            {
                throw new Exception("No default currency is set.");
            }

            var baseToDefaultQuery =
                CurrencyDataQueries.GetInverselyCalculatedExchangeRates(baseCurrencyCode, defaultCurrency.Code,
                    dateRange);
            var defaultToTargetQuery =
                CurrencyDataQueries.GetDirectExchangeRates(defaultCurrency.Code, targetCurrencyCode, dateRange);

            var baseToDefaultExchangeRates =
                await connection.QueryAsync<CurrencyExchangeRateDto>(baseToDefaultQuery.Query,
                    baseToDefaultQuery.Params);
            var defaultToTargetExchangeRates =
                await connection.QueryAsync<CurrencyExchangeRateDto>(defaultToTargetQuery.Query,
                    defaultToTargetQuery.Params);

            return _currencyConverter.CombineExchangeRates(baseToDefaultExchangeRates, defaultToTargetExchangeRates);
        }
    }
}
