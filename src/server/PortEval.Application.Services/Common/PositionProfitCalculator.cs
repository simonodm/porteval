﻿using PortEval.Application.Features.Interfaces.Calculators;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    public class PositionProfitCalculator : IPositionProfitCalculator
    {
        public decimal CalculateProfit(IEnumerable<PositionPriceRangeData> positionPriceRangeData, DateTime from, DateTime to)
        {
            if (from >= to)
            {
                return 0;
            }

            var realizedProfitAtRangeEnd = 0m;
            var unrealizedProfitAtRangeStart = 0m;
            var unrealizedProfitAtRangeEnd = 0m;

            foreach (var positionPriceData in positionPriceRangeData)
            {
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
            }

            return realizedProfitAtRangeEnd + unrealizedProfitAtRangeEnd -
                   unrealizedProfitAtRangeStart;
        }
    }
}
