using PortEval.Application.Features.Interfaces.Calculators;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common.Calculators
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
