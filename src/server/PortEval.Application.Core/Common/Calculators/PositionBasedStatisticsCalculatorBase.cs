using System;
using System.Collections.Generic;
using System.Linq;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Common.Calculators;

/// <summary>
///     A base class providing helper methods for calculation of positions' statistics.
/// </summary>
public abstract class PositionBasedStatisticsCalculatorBase
{
    /// <summary>
    ///     Converts <see cref="PositionPriceListData" /> to a collection of appropriate <see cref="PositionPriceRangeData" />
    ///     instances
    ///     for calculation of metrics in specified to-date intervals.
    /// </summary>
    /// <param name="positionPriceListData">A wrapper over a position's transactions and the underlying instrument's prices.</param>
    /// <param name="startTimes">
    ///     Start times of statistic intervals. Each of these intervals will be represented in the
    ///     returned collection.
    /// </param>
    /// <param name="endTime">End time of statistics intervals.</param>
    /// <returns>
    ///     A <see cref="List{T}" /> of <see cref="PositionPriceRangeData" /> corresponding to intervals between each time
    ///     provided in <paramref name="startTimes" />
    ///     and <paramref name="endTime" />
    /// </returns>
    protected List<PositionPriceRangeData> GetPriceRangeDataForStatistics(PositionPriceListData positionPriceListData,
        List<DateTime> startTimes, DateTime endTime)
    {
        var result = new List<PositionPriceRangeData>();

        using var priceEnumerator = positionPriceListData.Prices.GetEnumerator();
        priceEnumerator.MoveNext();

        // find the price at each time in startTimes
        var startPrices = startTimes
            .OrderBy(t => t)
            .Select(time => LimitDateToAvailableData(positionPriceListData, time))
            .Select(time => priceEnumerator.FindNextElementInEnumerator(t => t.Time <= time))
            .ToList();

        // populate missing start prices by taking the previous one
        // this handles situations when there are no prices between startTimes[i] and startTimes[i-1]
        for (var i = 1; i < startPrices.Count; i++) startPrices[i] ??= startPrices[i - 1];

        // do the same for the end price
        var endPrice = priceEnumerator.FindNextElementInEnumerator(p => p.Time <= endTime);
        if (endPrice == null)
        {
            endPrice = startPrices.LastOrDefault(p => p != null);
        }

        for (var i = 0; i < startTimes.Count; i++)
            result.Add(new PositionPriceRangeData
            {
                DateRange = new DateRangeParams { From = startTimes[i], To = endTime },
                PositionId = positionPriceListData.PositionId,
                PriceAtRangeEnd = endPrice,
                PriceAtRangeStart = startPrices[i],
                Transactions = positionPriceListData.Transactions
            });

        return result;
    }

    private DateTime LimitDateToAvailableData(PositionPriceListData positionPriceListData, DateTime date)
    {
        return date
            .GetMax(positionPriceListData.Transactions.FirstOrDefault()?.Time ?? DateTime.MinValue)
            .GetMax(positionPriceListData.Prices.FirstOrDefault()?.Time ?? DateTime.MinValue);
    }
}