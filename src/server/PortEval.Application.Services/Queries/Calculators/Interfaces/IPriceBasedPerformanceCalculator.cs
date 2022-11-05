namespace PortEval.Application.Services.Queries.Calculators.Interfaces
{
    internal interface IPriceBasedPerformanceCalculator
    {
        public decimal CalculatePerformance(decimal priceAtStart, decimal priceAtEnd);
    }
}
