﻿using System;
using System.Collections.Generic;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Common.Calculators;

/// <inheritdoc cref="IPositionStatisticsCalculator" />
public class PositionStatisticsCalculator : PositionBasedStatisticsCalculatorBase, IPositionStatisticsCalculator
{
    private readonly IPositionBreakEvenPointCalculator _breakEvenPointCalculator;
    private readonly IPositionPerformanceCalculator _performanceCalculator;
    private readonly IPositionProfitCalculator _profitCalculator;

    /// <summary>
    ///     Initializes the statistics calculator.
    /// </summary>
    public PositionStatisticsCalculator(IPositionProfitCalculator profitCalculator,
        IPositionPerformanceCalculator performanceCalculator,
        IPositionBreakEvenPointCalculator breakEvenPointCalculator)
    {
        _profitCalculator = profitCalculator;
        _performanceCalculator = performanceCalculator;
        _breakEvenPointCalculator = breakEvenPointCalculator;
    }

    /// <inheritdoc />
    public PositionStatisticsDto CalculateStatistics(
        PositionPriceListData positionPriceListData, DateTime time)
    {
        var totalRangeStart = DateTime.MinValue;
        var lastMonthRangeStart = time.AddMonths(-1);
        var lastWeekRangeStart = time.AddDays(-7);
        var lastDayRangeStart = time.AddDays(-1);

        var data = GetPriceRangeDataForStatistics(positionPriceListData, new List<DateTime>
        {
            totalRangeStart,
            lastMonthRangeStart,
            lastWeekRangeStart,
            lastDayRangeStart
        }, time);

        var result = new PositionStatisticsDto
        {
            Id = positionPriceListData.PositionId,
            TotalPerformance = _performanceCalculator.CalculatePerformance(new[] { data[0] }, totalRangeStart, time),
            LastMonthPerformance =
                _performanceCalculator.CalculatePerformance(new[] { data[1] }, lastMonthRangeStart, time),
            LastWeekPerformance =
                _performanceCalculator.CalculatePerformance(new[] { data[2] }, lastWeekRangeStart, time),
            LastDayPerformance =
                _performanceCalculator.CalculatePerformance(new[] { data[3] }, lastDayRangeStart, time),
            TotalProfit = _profitCalculator.CalculateProfit(new[] { data[0] }, totalRangeStart, time),
            LastMonthProfit = _profitCalculator.CalculateProfit(new[] { data[1] }, lastMonthRangeStart, time),
            LastWeekProfit = _profitCalculator.CalculateProfit(new[] { data[2] }, lastWeekRangeStart, time),
            LastDayProfit = _profitCalculator.CalculateProfit(new[] { data[3] }, lastDayRangeStart, time),
            BreakEvenPoint =
                _breakEvenPointCalculator.CalculatePositionBreakEvenPoint(positionPriceListData.Transactions)
        };

        return result;
    }
}