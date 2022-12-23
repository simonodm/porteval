using PortEval.Application.Features.Interfaces.Calculators;

namespace PortEval.Application.Features.Common
{
    /// <inheritdoc />
    public class InstrumentProfitCalculator : IInstrumentProfitCalculator
    {
        /// <inheritdoc />
        public decimal CalculateProfit(decimal priceAtStart, decimal priceAtEnd)
        {
            return priceAtEnd - priceAtStart;
        }
    }
}
