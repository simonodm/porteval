using System;
using System.Collections.Generic;
using System.Linq;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Common.Calculators;

/// <inheritdoc cref="IPortfolioStatisticsCalculator" />
public class PortfolioStatisticsCalculator : PositionBasedStatisticsCalculatorBase, IPortfolioStatisticsCalculator
{
    private readonly IPositionPerformanceCalculator _performanceCalculator;
    private readonly IPositionProfitCalculator _profitCalculator;

    /// <summary>
    ///     Initializes the statistics calculator.
    /// </summary>
    public PortfolioStatisticsCalculator(IPositionProfitCalculator profitCalculator,
        IPositionPerformanceCalculator performanceCalculator)
    {
        _profitCalculator = profitCalculator;
        _performanceCalculator = performanceCalculator;
    }

    /// <inheritdoc />
    public EntityStatisticsDto CalculateStatistics(
        PortfolioPositionsPriceListData portfolioData, DateTime time)
    {
        var totalRangeStart = DateTime.MinValue;
        var lastMonthRangeStart = time.AddMonths(-1);
        var lastWeekRangeStart = time.AddDays(-7);
        var lastDayRangeStart = time.AddDays(-1);

        var startTimes = new List<DateTime>
        {
            totalRangeStart,
            lastMonthRangeStart,
            lastWeekRangeStart,
            lastDayRangeStart
        };

        var data = portfolioData.PositionsPriceListData
            .Select(p => GetPriceRangeDataForStatistics(p, startTimes, time));

        var rangeData = startTimes
            .Select((_, i) => data.Select(d => d[i]))
            .ToList();


        var result = new EntityStatisticsDto
        {
            Id = portfolioData.PortfolioId,
            TotalPerformance = _performanceCalculator.CalculatePerformance(rangeData[0], DateTime.MinValue, time),
            LastMonthPerformance =
                _performanceCalculator.CalculatePerformance(rangeData[1], time.AddMonths(-1), time),
            LastWeekPerformance =
                _performanceCalculator.CalculatePerformance(rangeData[2], time.AddDays(-7), time),
            LastDayPerformance =
                _performanceCalculator.CalculatePerformance(rangeData[3], time.AddDays(-1), time),
            TotalProfit = _profitCalculator.CalculateProfit(rangeData[0], DateTime.MinValue, time),
            LastMonthProfit = _profitCalculator.CalculateProfit(rangeData[1], time.AddMonths(-1), time),
            LastWeekProfit = _profitCalculator.CalculateProfit(rangeData[2], time.AddDays(-7), time),
            LastDayProfit = _profitCalculator.CalculateProfit(rangeData[3], time.AddDays(-1), time)
        };

        return result;
    }
}