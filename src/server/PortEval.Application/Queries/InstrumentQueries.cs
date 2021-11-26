using Dapper;
using PortEval.Application.Models;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Application.Queries.DataQueries;
using PortEval.Application.Queries.Helpers;
using PortEval.Application.Queries.Interfaces;
using PortEval.Application.Queries.Models;
using PortEval.Domain.Models.Enums;
using PortEval.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Queries
{
    /// <inheritdoc cref="IInstrumentQueries"/>
    public class InstrumentQueries : IInstrumentQueries
    {
        private readonly PortEvalDbConnection _connection;
        private readonly ICurrencyExchangeRateQueries _exchangeRateQueries;

        public InstrumentQueries(PortEvalDbConnection connection, ICurrencyExchangeRateQueries exchangeRateQueries)
        {
            _connection = connection;
            _exchangeRateQueries = exchangeRateQueries;
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstruments"/>
        public async Task<QueryResponse<PaginatedResponse<InstrumentDto>>> GetInstruments(PaginationParams pagination)
        {
            var pageQuery = InstrumentDataQueries.GetInstrumentPageQuery(pagination);
            var totalCountQuery = InstrumentDataQueries.GetInstrumentCountQuery();

            using var connection = _connection.CreateConnection();

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

            using var connection = _connection.CreateConnection();
            var instrument = await connection.QueryFirstOrDefaultAsync<InstrumentDto>(query.Query, query.Params);

            return new QueryResponse<InstrumentDto>
            {
                Status = instrument != null ? QueryStatus.Ok : QueryStatus.NotFound,
                Response = instrument
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentPrices"/>
        public async Task<QueryResponse<PaginatedResponse<InstrumentPriceDto>>> GetInstrumentPrices(int instrumentId,
            PaginationParams pagination, DateRangeParams dateRange)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<PaginatedResponse<InstrumentPriceDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var pricePageQuery =
                InstrumentDataQueries.GetInstrumentPrices(instrumentId, dateRange.From, dateRange.To, pagination);
            var totalCountQuery =
                InstrumentDataQueries.GetInstrumentPriceCount(instrumentId, dateRange.From, dateRange.To);

            using var connection = _connection.CreateConnection();
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

        /// <inheritdoc cref="IInstrumentQueries.GetInstrumentPrice"/>
        public async Task<QueryResponse<InstrumentPriceDto>> GetInstrumentPrice(int instrumentId, DateTime time)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<InstrumentPriceDto>
                {
                    Status = QueryStatus.NotFound
                };
            }
            var query = InstrumentDataQueries.GetInstrumentPrice(instrumentId, time);

            using var connection = _connection.CreateConnection();
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
            if(instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<EntityProfitDto>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var firstPrice = (await GetInstrumentPrice(instrumentId, dateRange.From)).Response?.Price ?? 0;
            var secondPrice = (await GetInstrumentPrice(instrumentId, dateRange.To)).Response?.Price ?? 0;

            var profit = new EntityProfitDto
            {
                From = dateRange.From,
                To = dateRange.To,
                Profit = secondPrice - firstPrice
            };
            return new QueryResponse<EntityProfitDto>
            {
                Status = QueryStatus.Ok,
                Response = profit
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

            var performance = new EntityPerformanceDto
            {
                From = dateRange.From,
                To = dateRange.To,
                Performance = (secondPrice - firstPrice) / Math.Max(1, firstPrice)
            };

            return new QueryResponse<EntityPerformanceDto>
            {
                Status = QueryStatus.Ok,
                Response = performance
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentPrices"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPrices(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null)
        {
            var instrument = await GetInstrument(instrumentId);
            if(instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var process = new Func<DateTime, Task<EntityChartPointDto>>(async time =>
            {
                var price = await GetInstrumentPrice(instrumentId, time);
                if (price.Response == null) return null;

                price.Response.Time = time;
                return await _exchangeRateQueries.ConvertChartPointCurrency(instrument.Response.CurrencyCode, currencyCode,
                    EntityChartPointDto.FromInstrumentPrice(price.Response));
            });

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => await process(range.To)
            );

            var startPrice = await process(dateRange.From);
            if(startPrice != null)
            {
                calculations = calculations.Prepend(startPrice);
            }

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentProfit"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentProfit(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => await _exchangeRateQueries.ConvertChartPointCurrency(instrument.Response.CurrencyCode, currencyCode,
                    EntityChartPointDto.FromProfit((await GetInstrumentProfit(instrumentId, range.SetFrom(dateRange.From))).Response))
            );

            calculations = calculations.Prepend(new EntityChartPointDto(dateRange.From, 0));

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentPerformance"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPerformance(int instrumentId, DateRangeParams dateRange,
            AggregationFrequency frequency)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => EntityChartPointDto.FromPerformance((await GetInstrumentPerformance(instrumentId, range.SetFrom(dateRange.From))).Response)
            );

            calculations = calculations.Prepend(new EntityChartPointDto(dateRange.From, 0));

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentProfitAggregated"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentProfitAggregated(int instrumentId,
            DateRangeParams dateRange, AggregationFrequency frequency, string currencyCode = null)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => await _exchangeRateQueries.ConvertChartPointCurrency(instrument.Response.CurrencyCode, currencyCode,
                    EntityChartPointDto.FromProfit((await GetInstrumentProfit(instrumentId, range)).Response))
            );

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }

        /// <inheritdoc cref="IInstrumentQueries.ChartInstrumentPerformanceAggregated"/>
        public async Task<QueryResponse<IEnumerable<EntityChartPointDto>>> ChartInstrumentPerformanceAggregated(int instrumentId, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var instrument = await GetInstrument(instrumentId);
            if (instrument.Status == QueryStatus.NotFound)
            {
                return new QueryResponse<IEnumerable<EntityChartPointDto>>
                {
                    Status = QueryStatus.NotFound
                };
            }

            var calculations = await CalculationUtils.AggregateCalculations
            (
                dateRange,
                frequency,
                async range => EntityChartPointDto.FromPerformance((await GetInstrumentPerformance(instrumentId, range)).Response)
            );

            return new QueryResponse<IEnumerable<EntityChartPointDto>>
            {
                Status = QueryStatus.Ok,
                Response = calculations
            };
        }
    }
}
