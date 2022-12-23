using PortEval.Application.Features.Interfaces.Calculators;

namespace PortEval.Application.Features.Common
{
    public class InstrumentPerformanceCalculator : IInstrumentPerformanceCalculator
    {
        public decimal CalculatePerformance(decimal priceAtStart, decimal priceAtEnd)
        {
            if (priceAtStart == 0 && priceAtEnd > 0)
            {
                return 1;
            }

            if (priceAtStart == 0 && priceAtEnd == 0)
            {
                return 0;
            }

            return (priceAtEnd - priceAtStart) / priceAtStart;
        }
    }
}
