using PortEval.Application.Features.Interfaces.Calculators;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    /// <inheritdoc />
    public class PositionBreakEvenPointCalculator : IPositionBreakEvenPointCalculator
    {
        /// <inheritdoc />
        public decimal CalculatePositionBreakEvenPoint(IEnumerable<TransactionDto> transactions)
        {
            decimal totalPositionBuyingPrice = 0m;
            decimal positionAmount = 0m;

            foreach (var transaction in transactions)
            {
                totalPositionBuyingPrice += transaction.Amount * transaction.Price;
                positionAmount += transaction.Amount;
            }

            var bep = positionAmount != 0 ? totalPositionBuyingPrice / positionAmount : 0;

            return bep;
        }
    }
}
