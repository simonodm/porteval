namespace PortEval.Application.Services.Queries.Calculators.Interfaces
{
    internal interface IPriceBasedProfitCalculator
    {
        public decimal CalculateProfit(decimal priceAtStart, decimal priceAtEnd);
    }
}
