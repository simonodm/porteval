using PortEval.Application.Features.Interfaces.Calculators;

namespace PortEval.Application.Features.Common
{
    public class InstrumentProfitCalculator : IInstrumentProfitCalculator
    {
        public decimal CalculateProfit(decimal priceAtStart, decimal priceAtEnd)
        {
            return priceAtEnd - priceAtStart;
        }
    }
}
