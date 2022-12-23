namespace PortEval.Application.Features.Interfaces.Calculators
{
    public interface IInstrumentProfitCalculator
    {
        public decimal CalculateProfit(decimal priceAtStart, decimal priceAtEnd);
    }
}
