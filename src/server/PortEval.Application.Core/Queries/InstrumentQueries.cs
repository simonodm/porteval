using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Queries.DataQueries;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Queries
{
    /// <inheritdoc cref="IInstrumentQueries"/>
    public class InstrumentQueries : IInstrumentQueries
    {
        private readonly IDbConnectionCreator _connectionCreator;
        private readonly ICurrencyExchangeRateQueries _exchangeRateQueries;

        private readonly IInstrumentProfitCalculator _profitCalculator;
        private readonly IInstrumentPerformanceCalculator _performanceCalculator;

        private readonly ICurrencyConverter _currencyConverter;
        private readonly IInstrumentChartDataGenerator _chartGenerator;

        public InstrumentQueries(IDbConnectionCreator connection, ICurrencyExchangeRateQueries exchangeRateQueries,
            IInstrumentProfitCalculator profitCalculator, IInstrumentPerformanceCalculator performanceCalculator, ICurrencyConverter currentConverter,
            IInstrumentChartDataGenerator chartGenerator)
        {
            _connectionCreator = connection;
            _exchangeRateQueries = exchangeRateQueries;

            _profitCalculator = profitCalculator;
            _performanceCalculator = performanceCalculator;

            _currencyConverter = currentConverter;
            _chartGenerator = chartGenerator;
        }

        /// <inheritdoc cref="IInstrumentQueries.GetAllInstruments"/>
        public async Task<QueryResponse<IEnumerable<InstrumentDto>>> GetAllInstruments()
        {
            var query = InstrumentDataQueries.GetAllInstrumentsQuery();
            using var connection = _connectionCreator.CreateConnection();
            var instruments = await connection.QueryAsync<InstrumentDto>(query.Query, query.Params);

            return new QueryResponse<IEnumerable<InstrumentDto>>
            {
                Status = QueryStatus.Ok,
                Response = instruments
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentsPage"/>
        public async Task<QueryResponse<PaginatedResponse<InstrumentDto>>> GetInstrumentsPage(PaginationParams pagination)
        {
            var pageQuery = InstrumentDataQueries.GetInstrumentPageQuery(pagination);
            var totalCountQuery = InstrumentDataQueries.GetInstrumentCountQuery();

            using var connection = _connectionCreator.CreateConnection();

            var totalCount = await connection.QueryFirstAsync<int>(totalCountQuery.Query, totalCountQuery.Params);
            var data = (await connection.QueryAsync<InstrumentDto>(pageQuery.Query, pageQuery.Params)).ToList();

            var response = new PaginatedResponse<InstrumentDto>(pagination.Page, data.Count, totalCount, data);

            return new QueryResponse<PaginatedResponse<InstrumentDto>>
            {
                Status = QueryStatus.Ok,
                Response = response
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrument"/>
        public async Task<QueryResponse<InstrumentDto>> GetInstrument(int instrumentId)
        {
            var query = InstrumentDataQueries.GetInstrumentQuery(instrumentId);

            using var connection = _connectionCreator.CreateConnection();
            var instrument = await connection.QueryFirstOrDefaultAsync<InstrumentDto>(query.Query, query.Params);

            return new QueryResponse<InstrumentDto>
            {
                Status = instrument != null ? QueryStatus.Ok : QueryStatus.NotFound,
                Response = instrument
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentPrices(int, DateRangeParams)"/>
        public async Task<QueryResponse<IEnumerable<InstrumentPriceDto>>> GetInstrumentPrices(int instrumentId,
            DateRangeParams dateRange)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<InstrumentPriceDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            return await GetInstrumentPrices(instrument.Response, dateRange);
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentPrices(InstrumentDto, DateRangeParams)"/>
        public async Task<QueryResponse<IEnumerable<InstrumentPriceDto>>> GetInstrumentPrices(InstrumentDto instrument,
            DateRangeParams dateRange)
        {
            var query = InstrumentDataQueries.GetInstrumentPrices(instrument.Id, dateRange.From, dateRange.To);

            using var connection = _connectionCreator.CreateConnection();
            var prices = await connection.QueryAsync<InstrumentPriceDto>(query.Query, query.Params);

            return new QueryResponse<IEnumerable<InstrumentPriceDto>>
            {
                Status = QueryStatus.Ok,
                Response = prices
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentPricesPage"/>
        public async Task<QueryResponse<PaginatedResponse<InstrumentPriceDto>>> GetInstrumentPricesPage(int instrumentId,
            PaginationParams pagination, DateRangeParams dateRange, bool compressed = false, AggregationFrequency? frequency = null)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<PaginatedResponse<InstrumentPriceDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var pricePageQuery = compressed
                ? InstrumentDataQueries.GetInstrumentPricesPageCompressed(instrumentId, dateRange.From, dateRange.To, pagination, frequency)
                : InstrumentDataQueries.GetInstrumentPricesPage(instrumentId, dateRange.From, dateRange.To, pagination, frequency);
            var totalCountQuery = compressed
                ? InstrumentDataQueries.GetInstrumentPriceCompressedCount(instrumentId, dateRange.From, dateRange.To, frequency)
                : InstrumentDataQueries.GetInstrumentPriceCount(instrumentId, dateRange.From, dateRange.To, frequency);

            using var connection = _connectionCreator.CreateConnection();
            var totalCount = await connection.QueryFirstAsync<int>(totalCountQuery.Query, totalCountQuery.Params);
            var data = (await connection.QueryAsync<InstrumentPriceDto>(pricePageQuery.Query, pricePageQuery.Params))
                .ToList();

            var response = new PaginatedResponse<InstrumentPriceDto>(pagination.Page, data.Count, totalCount, data);

            return new QueryResponse<PaginatedResponse<InstrumentPriceDto>>
            {
                Status = QueryStatus.Ok,
                Response = response
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentSplits"/>
        public async Task<QueryResponse<IEnumerable<InstrumentSplitDto>>> GetInstrumentSplits(int instrumentId)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status != QueryStatus.Ok)
            {
                return new QueryResponse<IEnumerable<InstrumentSplitDto>>
                {
                    Status = instrument.Status
                };
            }

            var query = InstrumentDataQueries.GetInstrumentSplits(instrumentId);

            using var connection = _connectionCreator.CreateConnection();
            var splits = await connection.QueryAsync<InstrumentSplitDto>(query.Query, query.Params);

            return new QueryResponse<IEnumerable<InstrumentSplitDto>>
            {
                Status = QueryStatus.Ok,
                Response = splits
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentSplit"/>
        public async Task<QueryResponse<InstrumentSplitDto>> GetInstrumentSplit(int instrumentId, int splitId)
        {
            var query = InstrumentDataQueries.GetInstrumentSplit(instrumentId, splitId);
            using var connection = _connectionCreator.CreateConnection();
            var split = await connection.QueryFirstOrDefaultAsync<InstrumentSplitDto>(query.Query, query.Params);

            return new QueryResponse<InstrumentSplitDto>
            {
                Status = split != null ? QueryStatus.Ok : QueryStatus.NotFound,
                Response = split
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentPrice"/>
        public async Task<QueryResponse<InstrumentPriceDto>> GetInstrumentPrice(int instrumentId, DateTime time)
        {
            var query = InstrumentDataQueries.GetInstrumentPrice(instrumentId, time);

            using var connection = _connectionCreator.CreateConnection();
            var price = await connection.QueryFirstOrDefaultAsync<InstrumentPriceDto>(query.Query, query.Params);

            return new QueryResponse<InstrumentPriceDto>
            {
                Status = QueryStatus.Ok,
                Response = price
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentProfit"/>
        public async Task<QueryResponse<EntityProfitDto>> GetInstrumentProfit(int instrumentId, DateRangeParams dateRange)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<EntityProfitDto>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstPrice = (await GetInstrumentPrice(instrumentId, dateRange.From)).Response?.Price ?? 0;
            var secondPrice = (await GetInstrumentPrice(instrumentId, dateRange.To)).Response?.Price ?? 0;

            var profit = _profitCalculator.CalculateProfit(firstPrice, secondPrice);

            var result = new EntityProfitDto
            {
                From = dateRange.From,
                To = dateRange.To,
                Profit = profit,
                CurrencyCode = instrument.Response.CurrencyCode
            };
            return new QueryResponse<EntityProfitDto>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentPerformance"/>
        public async Task<QueryResponse<EntityPerformanceDto>> GetInstrumentPerformance(int instrumentId, DateRangeParams dateRange)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<EntityPerformanceDto>
                {
                    Status = QueryStatus.NotFound
                };
            }
            var firstPrice = (await GetInstrumentPrice(instrumentId, dateRange.From)).Response?.Price ?? 0;
            var secondPrice = (await GetInstrumentPrice(instrumentId, dateRange.To)).Response?.Price ?? 0;

            var performance = _performanceCalculator.CalculatePerformance(firstPrice, secondPrice);

            var result = new EntityPerformanceDto
            {
                From = dateRange.From,
                To = dateRange.To,
                Performance = performance
            };

            return new QueryResponse<EntityPerformanceDto>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentPrices"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPrices(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null)
        {
            var instrumentResponse = await GetInstrument(instrumentId);
            if (instrumentResponse.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var pricesResponse = await GetInstrumentPrices(instrumentResponse.Response, dateRange.SetFrom(DateTime.MinValue));
            var result = _chartGenerator.ChartPrices(pricesResponse.Response, dateRange, frequency);

            if (!string.Equals(instrumentResponse.Response.CurrencyCode, currencyCode, StringComparison.CurrentCultureIgnoreCase))
            {
                var exchangeRatesResponse =
                    await _exchangeRateQueries.GetExchangeRates(instrumentResponse.Response.CurrencyCode, currencyCode, dateRange.SetFrom(DateTime.MinValue));
                result = _currencyConverter.ConvertChartPoints(result, exchangeRatesResponse.Response);
            }

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentProfit"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentProfit(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null)
        {
            var instrumentResponse = await GetInstrument(instrumentId);
            if (instrumentResponse.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var pricesResponse = await GetInstrumentPrices(instrumentResponse.Response, dateRange.SetFrom(DateTime.MinValue));
            var result = _chartGenerator.ChartProfit(pricesResponse.Response, dateRange, frequency);

            if (!string.Equals(instrumentResponse.Response.CurrencyCode, currencyCode, StringComparison.CurrentCultureIgnoreCase))
            {
                var exchangeRatesResponse =
                    await _exchangeRateQueries.GetExchangeRates(instrumentResponse.Response.CurrencyCode, currencyCode, dateRange.SetFrom(DateTime.MinValue));
                result = _currencyConverter.ConvertChartPoints(result, exchangeRatesResponse.Response);
            }

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentPerformance"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPerformance(int instrumentId, DateRangeParams dateRange,
            AggregationFrequency frequency)
        {
            var instrumentResponse = await GetInstrument(instrumentId);
            if (instrumentResponse.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var pricesResponse = await GetInstrumentPrices(instrumentResponse.Response, dateRange.SetFrom(DateTime.MinValue));
            var result = _chartGenerator.ChartPerformance(pricesResponse.Response, dateRange, frequency);

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentProfitAggregated"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentProfitAggregated(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null)
        {
            var instrumentResponse = await GetInstrument(instrumentId);
            if (instrumentResponse.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var pricesResponse = await GetInstrumentPrices(instrumentResponse.Response, dateRange.SetFrom(DateTime.MinValue));
            var result = _chartGenerator.ChartAggregatedProfit(pricesResponse.Response, dateRange, frequency);

            if (!string.Equals(instrumentResponse.Response.CurrencyCode, currencyCode, StringComparison.CurrentCultureIgnoreCase))
            {
                var exchangeRatesResponse =
                    await _exchangeRateQueries.GetExchangeRates(instrumentResponse.Response.CurrencyCode, currencyCode, dateRange.SetFrom(DateTime.MinValue));
                result = _currencyConverter.ConvertChartPoints(result, exchangeRatesResponse.Response);
            }

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentPerformanceAggregated"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPerformanceAggregated(int instrumentId, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var instrumentResponse = await GetInstrument(instrumentId);
            if (instrumentResponse.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var pricesResponse = await GetInstrumentPrices(instrumentResponse.Response, dateRange.SetFrom(DateTime.MinValue));
            var result = _chartGenerator.ChartAggregatedPerformance(pricesResponse.Response, dateRange, frequency);

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = result
            };
        }

    }
}
