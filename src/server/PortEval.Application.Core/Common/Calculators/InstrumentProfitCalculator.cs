using PortEval.Application.Core.Interfaces.Calculators;

namespace PortEval.Application.Core.Common.Calculators;

/// <inheritdoc />
public class InstrumentProfitCalculator : IInstrumentProfitCalculator
{
    /// <inheritdoc />
    public decimal CalculateProfit(decimal priceAtStart, decimal priceAtEnd)
    {
        return priceAtEnd - priceAtStart;
    }
}