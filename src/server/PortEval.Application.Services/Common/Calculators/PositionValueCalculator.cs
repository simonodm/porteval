﻿using PortEval.Application.Features.Interfaces.Calculators;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common.Calculators
{
    /// <inheritdoc />
    public class PositionValueCalculator : IPositionValueCalculator
    {
        /// <inheritdoc />
        public decimal CalculateValue(IEnumerable<PositionPriceRangeData> positionsPriceData, DateTime time)
        {
            var value = 0m;
            foreach (var positionPriceData in positionsPriceData)
            {
                foreach (var transaction in positionPriceData.Transactions)
                {
                    if (transaction.Time <= time)
                    {
                        value += transaction.Amount * positionPriceData.PriceAtRangeEnd?.Price ?? 0m;
                    }
                }
            }

            return value;
        }
    }
}
