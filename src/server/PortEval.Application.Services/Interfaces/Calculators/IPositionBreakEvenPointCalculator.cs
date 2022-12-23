using PortEval.Application.Models.DTOs;
using System.Collections.Generic;

namespace PortEval.Application.Features.Interfaces.Calculators
{
    public interface IPositionBreakEvenPointCalculator
    {
        public decimal CalculatePositionBreakEvenPoint(IEnumerable<TransactionDto> transactions);
    }
}
