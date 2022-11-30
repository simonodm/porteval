using PortEval.Application.Services.Queries.Calculators.Interfaces;

namespace PortEval.Application.Services.Queries.Calculators
{
    internal class PriceBasedProfitCalculator : IPriceBasedProfitCalculator
    {
        public decimal CalculateProfit(decimal priceAtStart, decimal priceAtEnd)
        {
            return priceAtEnd - priceAtStart;
        }
    }
}
