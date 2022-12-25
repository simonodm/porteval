using PortEval.Application.Features.Interfaces.Calculators;
using PortEval.Application.Features.Interfaces.ChartDataGenerators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using PortEval.Application.Features.Extensions;

namespace PortEval.Application.Features.Common.ChartDataGenerators
{
    /// <inheritdoc cref="IInstrumentChartDataGenerator"/>
    public class InstrumentChartDataGenerator : ChartDataGeneratorBase, IInstrumentChartDataGenerator
    {
        private readonly IInstrumentProfitCalculator _profitCalculator;
        private readonly IInstrumentPerformanceCalculator _performanceCalculator;

        public InstrumentChartDataGenerator(IInstrumentProfitCalculator profitCalculator,
            IInstrumentPerformanceCalculator performanceCalculator)
        {
            _profitCalculator = profitCalculator;
            _performanceCalculator = performanceCalculator;
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartPrices(IEnumerable<InstrumentPriceDto> prices, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var adjustedDateRange = LimitDateRangeToFirstInstrumentPrice(prices, dateRange);
            var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);

            // prepend a range to calculate value at chart start as well
            ranges = ranges.Prepend(new DateRangeParams
            {
                From = adjustedDateRange.From,
                To = adjustedDateRange.From
            });

            return BuildChartFromPrices(prices, ranges, price => price.PriceAtRangeEnd);
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartProfit(IEnumerable<InstrumentPriceDto> prices, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var adjustedDateRange = LimitDateRangeToFirstInstrumentPrice(prices, dateRange);
            var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);

            // prepend a range to calculate value at chart start as well
            ranges = ranges.Prepend(new DateRangeParams
            {
                From = adjustedDateRange.From,
                To = adjustedDateRange.From
            });

            var startPrice = prices.LastOrDefault(p => p.Time <= adjustedDateRange.From)?.Price ?? 0m;

            var result = BuildChartFromPrices(prices, ranges, price =>
                _profitCalculator.CalculateProfit(startPrice, price.PriceAtRangeEnd)
            );

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartPerformance(IEnumerable<InstrumentPriceDto> prices, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var adjustedDateRange = LimitDateRangeToFirstInstrumentPrice(prices, dateRange);
            var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);

            // prepend a range to calculate value at chart start as well
            ranges = ranges.Prepend(new DateRangeParams
            {
                From = adjustedDateRange.From,
                To = adjustedDateRange.From
            });

            var startPrice = prices.LastOrDefault(p => p.Time <= adjustedDateRange.From)?.Price ?? 0m;

            var result = BuildChartFromPrices(prices, ranges, price =>
                _performanceCalculator.CalculatePerformance(startPrice, price.PriceAtRangeEnd)
            );

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartAggregatedProfit(IEnumerable<InstrumentPriceDto> prices, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var adjustedDateRange = LimitDateRangeToFirstInstrumentPrice(prices, dateRange);
            var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);

            return BuildChartFromPrices(prices, ranges, price =>
            {
                var profit = _profitCalculator.CalculateProfit(price.PriceAtRangeStart, price.PriceAtRangeEnd);
                return profit;
            });
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ChartAggregatedPerformance(IEnumerable<InstrumentPriceDto> prices, DateRangeParams dateRange, AggregationFrequency frequency)
        {
            var adjustedDateRange = LimitDateRangeToFirstInstrumentPrice(prices, dateRange);
            var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);

            return BuildChartFromPrices(prices, ranges, price =>
            {
                var performance = _performanceCalculator.CalculatePerformance(price.PriceAtRangeStart, price.PriceAtRangeEnd);
                return performance;
            });
        }

        private IEnumerable<EntityChartPointDto> BuildChartFromPrices(IEnumerable<InstrumentPriceDto> prices,
            IEnumerable<DateRangeParams> ranges, Func<InstrumentPriceRangeData, decimal> callback)
        {
            using var dataPointGenerator = new InstrumentRangeDataGenerator(prices, ranges);

            var result = new List<EntityChartPointDto>();

            while (!dataPointGenerator.IsFinished())
            {
                var current = dataPointGenerator.GetNextRangeData();
                if (current != null)
                {
                    var chartPointValue = callback(current);
                    result.Add(new EntityChartPointDto(current.DateRange.To, chartPointValue));
                }
            }

            return result;
        }

        private DateRangeParams LimitDateRangeToFirstInstrumentPrice(IEnumerable<InstrumentPriceDto> prices,
            DateRangeParams originalRange)
        {
            var firstPrice = prices.FirstOrDefault();
            if (firstPrice != null)
            {
                return originalRange.SetFrom(originalRange.From.GetMax(firstPrice.Time));
            }

            return originalRange;
        }
    }
}
