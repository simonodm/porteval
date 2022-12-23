namespace PortEval.Application.Features.Interfaces.Calculators
{
    public interface IInstrumentPerformanceCalculator
    {
        public decimal CalculatePerformance(decimal priceAtStart, decimal priceAtEnd);
    }
}
