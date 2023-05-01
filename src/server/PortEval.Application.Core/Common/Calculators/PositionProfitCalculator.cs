using System;
using System.Collections.Generic;
using PortEval.Application.Core.Interfaces.Calculators;

namespace PortEval.Application.Core.Common.Calculators;

/// <inheritdoc />
public class PositionProfitCalculator : IPositionProfitCalculator
{
    /// <inheritdoc />
    public decimal CalculateProfit(IEnumerable<PositionPriceRangeData> positionPriceRangeData, DateTime from,
        DateTime to)
    {
        if (from >= to)
        {
            return 0;
        }

        var realizedProfitAtRangeEnd = 0m;
        var unrealizedProfitAtRangeStart = 0m;
        var unrealizedProfitAtRangeEnd = 0m;

        foreach (var positionPriceData in positionPriceRangeData)
        foreach (var transaction in positionPriceData.Transactions)
        {
            unrealizedProfitAtRangeEnd +=
                transaction.Amount * positionPriceData.PriceAtRangeEnd?.Price ?? 0m;

            if (transaction.Time < from)
            {
                unrealizedProfitAtRangeStart += transaction.Amount * positionPriceData.PriceAtRangeStart?.Price ?? 0m;
            }
            else if (transaction.Time <= to)
            {
                realizedProfitAtRangeEnd -= transaction.Amount * transaction.Price;
            }
            else
            {
                break;
            }
        }

        return realizedProfitAtRangeEnd + unrealizedProfitAtRangeEnd -
               unrealizedProfitAtRangeStart;
    }
}