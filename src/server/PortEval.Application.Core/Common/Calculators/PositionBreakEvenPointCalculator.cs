using System.Collections.Generic;
using PortEval.Application.Core.Interfaces.Calculators;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Common.Calculators
{
    /// <inheritdoc />
    public class PositionBreakEvenPointCalculator : IPositionBreakEvenPointCalculator
    {
        /// <inheritdoc />
        public decimal CalculatePositionBreakEvenPoint(IEnumerable<TransactionDto> transactions)
        {
            decimal realizedProfit = 0m;
            decimal positionAmount = 0m;

            foreach (var transaction in transactions)
            {
                realizedProfit += transaction.Amount * transaction.Price;
                positionAmount += transaction.Amount;
            }

            var bep = positionAmount != 0 ? realizedProfit / positionAmount : 0;

            return bep;
        }
    }
}
