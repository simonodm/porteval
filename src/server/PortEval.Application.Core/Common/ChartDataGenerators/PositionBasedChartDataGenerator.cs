using System;
using System.Collections.Generic;
using System.Linq;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Common.ChartDataGenerators;

/// <summary>
///     An abstract class providing chart data generation functionality for position-based entities.
/// </summary>
public abstract class PositionBasedChartDataGenerator : ChartDataGeneratorBase
{
    /// <summary>
    ///     Position performance calculator.
    /// </summary>
    protected readonly IPositionPerformanceCalculator PerformanceCalculator;

    /// <summary>
    ///     Position profit calculator.
    /// </summary>
    protected readonly IPositionProfitCalculator ProfitCalculator;

    /// <summary>
    ///     Position value calculator.
    /// </summary>
    protected readonly IPositionValueCalculator ValueCalculator;

    /// <summary>
    ///     Initializes the chart data generator.
    /// </summary>
    protected PositionBasedChartDataGenerator(IPositionValueCalculator valueCalculator,
        IPositionProfitCalculator profitCalculator, IPositionPerformanceCalculator performanceCalculator)
    {
        ValueCalculator = valueCalculator;
        ProfitCalculator = profitCalculator;
        PerformanceCalculator = performanceCalculator;
    }

    /// <summary>
    ///     Generates price chart data based on the provided collection of position(s) data.
    /// </summary>
    /// <param name="positionsPriceListData">
    ///     A collection of positions' price list data, see <see cref="PositionPriceListData" />.
    ///     If this parameter contains data for more than one position, then the data is aggregated into a single line.
    ///     This is done to enable portfolio line generation using the same interface.
    /// </param>
    /// <param name="dateRange">Date range of the chart.</param>
    /// <param name="frequency">Chart point interval</param>
    /// <returns>An <see cref="IEnumerable{T}" /> containing chart line points.</returns>
    protected IEnumerable<EntityChartPointDto> ChartValue(IEnumerable<PositionPriceListData> positionsPriceListData,
        DateRangeParams dateRange, AggregationFrequency frequency)
    {
        var adjustedDateRange = LimitDateRangeToFirstTransaction(positionsPriceListData, dateRange);
        var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);

        ranges = ranges.Prepend(new DateRangeParams
        {
            From = adjustedDateRange.From,
            To = adjustedDateRange.From
        });

        var result = BuildChartFromPositionData(
            positionsPriceListData,
            ranges,
            (data, range) => ValueCalculator.CalculateValue(data, range.To)
        );

        return result;
    }

    /// <summary>
    ///     Generates profit chart data based on the provided collection of position(s) data.
    /// </summary>
    /// <param name="positionsPriceListData">
    ///     A collection of positions' price list data, see <see cref="PositionPriceListData" />.
    ///     If this parameter contains data for more than one position, then the data is aggregated into a single line.
    ///     This is done to enable portfolio line generation using the same interface.
    /// </param>
    /// <param name="dateRange">Date range of the chart.</param>
    /// <param name="frequency">Chart point interval</param>
    /// <returns>An <see cref="IEnumerable{T}" /> containing chart line points.</returns>
    protected IEnumerable<EntityChartPointDto> ChartProfit(IEnumerable<PositionPriceListData> positionsPriceListData,
        DateRangeParams dateRange, AggregationFrequency frequency)
    {
        var adjustedDateRange = LimitDateRangeToFirstTransaction(positionsPriceListData, dateRange);
        var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);
        ranges = ranges.Prepend(new DateRangeParams { From = adjustedDateRange.From, To = adjustedDateRange.From });

        var startPrices = positionsPriceListData.ToDictionary(p => p.PositionId,
            p => FindPriceAtRangeStart(p, adjustedDateRange));

        var result = BuildChartFromPositionData(
            positionsPriceListData,
            ranges,
            (data, range) =>
            {
                var rangeData = data.Select(p => new PositionPriceRangeData
                {
                    DateRange = p.DateRange,
                    PositionId = p.PositionId,
                    PriceAtRangeEnd = p.PriceAtRangeEnd,
                    PriceAtRangeStart = startPrices[p.PositionId],
                    Transactions = p.Transactions
                });
                return ProfitCalculator.CalculateProfit(rangeData, adjustedDateRange.From, range.To);
            });

        return result;
    }

    /// <summary>
    ///     Generates performance chart data based on the provided collection of position(s) data.
    /// </summary>
    /// <param name="positionsPriceListData">
    ///     A collection of positions' price list data, see <see cref="PositionPriceListData" />.
    ///     If this parameter contains data for more than one position, then the data is aggregated into a single line.
    ///     This is done to enable portfolio line generation using the same interface.
    /// </param>
    /// <param name="dateRange">Date range of the chart.</param>
    /// <param name="frequency">Chart point interval</param>
    /// <returns>An <see cref="IEnumerable{T}" /> containing chart line points.</returns>
    protected IEnumerable<EntityChartPointDto> ChartPerformance(
        IEnumerable<PositionPriceListData> positionsPriceListData, DateRangeParams dateRange,
        AggregationFrequency frequency)
    {
        var adjustedDateRange = LimitDateRangeToFirstTransaction(positionsPriceListData, dateRange);
        var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);
        ranges = ranges.Prepend(new DateRangeParams { From = adjustedDateRange.From, To = adjustedDateRange.From });

        var startPrices = positionsPriceListData.ToDictionary(p => p.PositionId,
            p => FindPriceAtRangeStart(p, adjustedDateRange));

        var result = BuildChartFromPositionData(
            positionsPriceListData,
            ranges,
            (data, range) =>
            {
                var rangeData = data.Select(p => new PositionPriceRangeData
                {
                    DateRange = p.DateRange,
                    PositionId = p.PositionId,
                    PriceAtRangeEnd = p.PriceAtRangeEnd,
                    PriceAtRangeStart = startPrices[p.PositionId],
                    Transactions = p.Transactions
                });
                return PerformanceCalculator.CalculatePerformance(rangeData, adjustedDateRange.From, range.To);
            });

        return result;
    }

    /// <summary>
    ///     Generates aggregated profit chart data based on the provided collection of position(s) data.
    /// </summary>
    /// <param name="positionsPriceListData">
    ///     A collection of positions' price list data, see <see cref="PositionPriceListData" />.
    ///     If this parameter contains data for more than one position, then the data is aggregated into a single line.
    ///     This is done to enable portfolio line generation using the same interface.
    /// </param>
    /// <param name="dateRange">Date range of the chart.</param>
    /// <param name="frequency">Chart point interval</param>
    /// <returns>An <see cref="IEnumerable{T}" /> containing chart line points.</returns>
    protected IEnumerable<EntityChartPointDto> ChartAggregatedProfit(
        IEnumerable<PositionPriceListData> positionsPriceListData, DateRangeParams dateRange,
        AggregationFrequency frequency)
    {
        var adjustedDateRange = LimitDateRangeToFirstTransaction(positionsPriceListData, dateRange);
        var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);
        var result = BuildChartFromPositionData(
            positionsPriceListData,
            ranges,
            (data, range) => ProfitCalculator.CalculateProfit(data, range.From, range.To)
        );

        return result;
    }

    /// <summary>
    ///     Generates aggregated performance chart data based on the provided collection of position(s) data.
    /// </summary>
    /// <param name="positionsPriceListData">
    ///     A collection of positions' price list data, see <see cref="PositionPriceListData" />.
    ///     If this parameter contains data for more than one position, then the data is aggregated into a single line.
    ///     This is done to enable portfolio line generation using the same interface.
    /// </param>
    /// <param name="dateRange">Date range of the chart.</param>
    /// <param name="frequency">Chart point interval</param>
    /// <returns>An <see cref="IEnumerable{T}" /> containing chart line points.</returns>
    protected IEnumerable<EntityChartPointDto> ChartAggregatedPerformance(
        IEnumerable<PositionPriceListData> positionsPriceListData, DateRangeParams dateRange,
        AggregationFrequency frequency)
    {
        var adjustedDateRange = LimitDateRangeToFirstTransaction(positionsPriceListData, dateRange);
        var ranges = GetAggregatedRanges(adjustedDateRange.From, adjustedDateRange.To, frequency);
        var result = BuildChartFromPositionData(
            positionsPriceListData,
            ranges,
            (data, range) => PerformanceCalculator.CalculatePerformance(data, range.From, range.To)
        );

        return result;
    }

    private IEnumerable<EntityChartPointDto> BuildChartFromPositionData(
        IEnumerable<PositionPriceListData> positionsPriceListData,
        IEnumerable<DateRangeParams> ranges,
        Func<IEnumerable<PositionPriceRangeData>, DateRangeParams, decimal> callback)
    {
        var result = new List<EntityChartPointDto>();

        var positionChartPointGenerators = positionsPriceListData
            .Select(p => new PositionRangeDataGenerator(p, ranges))
            .ToList();

        foreach (var range in ranges)
        {
            // a collection of all positions' data needed to generate the current chart point
            var chartPointDataList = new List<PositionPriceRangeData>();

            // iterate through generators and populate the previous collection
            foreach (var generator in positionChartPointGenerators)
            {
                var nextChartPointData = generator.GetNextRangeData();
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

        foreach (var generator in positionChartPointGenerators) generator.Dispose();

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

    private InstrumentPriceDto FindPriceAtRangeStart(PositionPriceListData priceListData, DateRangeParams dateRange)
    {
        return priceListData.Prices.LastOrDefault(p => p.Time <= dateRange.From);
    }
}