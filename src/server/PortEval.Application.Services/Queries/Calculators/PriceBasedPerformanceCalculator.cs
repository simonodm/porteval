using PortEval.Application.Services.Queries.Calculators.Interfaces;

namespace PortEval.Application.Services.Queries.Calculators
{
    internal class PriceBasedPerformanceCalculator : IPriceBasedPerformanceCalculator
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
