using PortEval.Application.Models.DTOs;
using System.Collections.Generic;

namespace PortEval.Application.Core.Interfaces.Calculators
{
    /// <summary>
    /// Calculates a position's break-even point.
    /// </summary>
    public interface IPositionBreakEvenPointCalculator
    {
        /// <summary>
        /// Calculates a position's break-even point based on its transactions.
        /// </summary>
        /// <param name="transactions">Position's transactions</param>
        /// <returns>The position's break-even point.</returns>
        public decimal CalculatePositionBreakEvenPoint(IEnumerable<TransactionDto> transactions);
    }
}
