using PortEval.Application.Features.Interfaces.Calculators;
using PortEval.Application.Features.Interfaces.ChartDataGenerators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using PortEval.Application.Features.Extensions;

namespace PortEval.Application.Features.Common
{
    /// <inheritdoc cref="IPositionChartDataGenerator"/>
    public class PositionChartDataGenerator : ChartDataGeneratorBase, IPositionChartDataGenerator
    {
        private readonly IPositionValueCalculator _valueCalculator;
        private readonly IPositionProfitCalculator _profitCalculator;
        private readonly IPositionPerformanceCalculator _performanceCalculator;

        public PositionChartDataGenerator(IPositionValueCalculator valueCalculator,
            IPositionProfitCalculator profitCalculator, IPositionPerformanceCalculator performanceCalculator)
        {
            _valueCalculator = valueCalculator;
            _profitCalculator = profitCalculator;
            _performanceCalculator = performanceCalculator;
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartValue(IEnumerable<PositionPriceListData> positionsPriceListData,
            DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var adjustedDateRange = LimitDateRangeToFirstTransaction(positionsPriceListData, dateRange);
            var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);

            ranges = ranges.Prepend(new DateRangeParams
            {
                From = adjustedDateRange.From,
                To = adjustedDateRange.From
            });

            var result = BuildChartFromPositionData(positionsPriceListData, ranges, (data, range) =>
            {
                return _valueCalculator.CalculateValue(data.Select(p => new PositionPriceData
                {
                    Price = p.InstrumentPriceAtRangeEnd,
                    Time = range.To,
                    Transactions = p.TransactionsToProcess
                }), range.To);
            });

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartProfit(IEnumerable<PositionPriceListData> positionsPriceListData, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var adjustedDateRange = LimitDateRangeToFirstTransaction(positionsPriceListData, dateRange);
            var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);
            ranges = ranges.Prepend(new DateRangeParams { From = adjustedDateRange.From, To = adjustedDateRange.From });

            var result = BuildChartFromPositionData(positionsPriceListData, ranges, (data, range) =>
            {
                return _profitCalculator.CalculateProfit(data.Select(p => new PositionPriceRangeData
                {
                    PriceAtRangeStart = p.StartPrice,
                    PriceAtRangeEnd = p.InstrumentPriceAtRangeEnd,
                    Transactions = p.TransactionsToProcess
                }), adjustedDateRange.From, range.To);
            });
            
            return result;
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartPerformance(IEnumerable<PositionPriceListData> positionsPriceListData, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var adjustedDateRange = LimitDateRangeToFirstTransaction(positionsPriceListData, dateRange);
            var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);
            ranges = ranges.Prepend(new DateRangeParams { From = adjustedDateRange.From, To = adjustedDateRange.From });
            var result = BuildChartFromPositionData(positionsPriceListData, ranges, (data, range) =>
            {
                return _performanceCalculator.CalculatePerformance(data.Select(p => new PositionPriceRangeData
                {
                    PriceAtRangeStart = p.StartPrice,
                    PriceAtRangeEnd = p.InstrumentPriceAtRangeEnd,
                    Transactions = p.TransactionsToProcess
                }), adjustedDateRange.From, range.To);
            });

            // prepend initial state
            return result;
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartAggregatedProfit(IEnumerable<PositionPriceListData> positionsPriceListData, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var adjustedDateRange = LimitDateRangeToFirstTransaction(positionsPriceListData, dateRange);
            var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);
            var result = BuildChartFromPositionData(positionsPriceListData, ranges, (data, range) =>
            {
                return _profitCalculator.CalculateProfit(data.Select(p => new PositionPriceRangeData
                {
                    PriceAtRangeStart = p.InstrumentPriceAtRangeStart,
                    PriceAtRangeEnd = p.InstrumentPriceAtRangeEnd,
                    Transactions = p.TransactionsToProcess
                }), range.From, range.To);
            });

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartAggregatedPerformance(IEnumerable<PositionPriceListData> positionsPriceListData, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var adjustedDateRange = LimitDateRangeToFirstTransaction(positionsPriceListData, dateRange);
            var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);
            var result = BuildChartFromPositionData(positionsPriceListData, ranges, (data, range) =>
            {
                return _performanceCalculator.CalculatePerformance(data.Select(p => new PositionPriceRangeData
                {
                    PriceAtRangeStart = p.InstrumentPriceAtRangeStart,
                    PriceAtRangeEnd = p.InstrumentPriceAtRangeEnd,
                    Transactions = p.TransactionsToProcess
                }), range.From, range.To);
            });

            return result;
        }

        private IEnumerable<EntityChartPointDto> BuildChartFromPositionData(IEnumerable<PositionPriceListData> positionsPriceListData,
            IEnumerable<DateRangeParams> ranges, Func<IEnumerable<PositionChartPointData>, DateRangeParams, decimal> callback)
        {
            var result = new List<EntityChartPointDto>();

            var positionChartPointGenerators = positionsPriceListData
                .Select(p => new PositionChartPointGenerator(p, ranges))
                .ToList();

            foreach (var range in ranges)
            {
                // a collection of all positions' data needed to generate the current chart point
                var chartPointDataList = new List<PositionChartPointData>();

                // iterate through generators and populate the previous collection
                foreach (var generator in positionChartPointGenerators)
                {
                    var nextChartPointData = generator.GetNextChartPointData();
                    if (nextChartPointData != null)
                    {
                        chartPointDataList.Add(nextChartPointData);
                    }
                }

                if (chartPointDataList.Count > 0)
                {
                    // calculate chart point value based on the callback and add it to result
                    var value = callback(chartPointDataList, range);
                    result.Add(new EntityChartPointDto(range.To, value));
                }
            }

            foreach (var generator in positionChartPointGenerators)
            {
                generator.Dispose();
            }

            return result;
        }

        private DateRangeParams LimitDateRangeToFirstTransaction(
            IEnumerable<PositionPriceListData> positionsPriceListData, DateRangeParams originalDateRange)
        {
            var min = DateTime.MaxValue;

            foreach (var data in positionsPriceListData)
            {
                var firstTransaction = data.Transactions.FirstOrDefault();
                if (firstTransaction != null && firstTransaction.Time < min)
                {
                    min = firstTransaction.Time;
                }
            }

            return originalDateRange.SetFrom(originalDateRange.From.GetMax(min));
        }

        private decimal FindPriceAtRangeStart(PositionPriceListData priceListData, DateRangeParams dateRange)
        {
            return priceListData.Prices.LastOrDefault(p => p.Time <= dateRange.From)?.Price ?? 0m;
        }
    }
}
