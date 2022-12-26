using PortEval.Application.Features.Interfaces.Calculators;
using PortEval.Application.Models.DTOs;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common.Calculators
{
    public class PositionStatisticsCalculator : PositionBasedStatisticsCalculatorBase, IPositionStatisticsCalculator
    {
        private readonly IPositionProfitCalculator _profitCalculator;
        private readonly IPositionPerformanceCalculator _performanceCalculator;
        private readonly IPositionBreakEvenPointCalculator _breakEvenPointCalculator;

        public PositionStatisticsCalculator(IPositionProfitCalculator profitCalculator,
            IPositionPerformanceCalculator performanceCalculator, IPositionBreakEvenPointCalculator breakEvenPointCalculator)
        {
            _profitCalculator = profitCalculator;
            _performanceCalculator = performanceCalculator;
            _breakEvenPointCalculator = breakEvenPointCalculator;
        }

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
}
