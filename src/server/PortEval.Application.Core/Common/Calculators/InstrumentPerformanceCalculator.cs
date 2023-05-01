using PortEval.Application.Core.Interfaces.Calculators;

namespace PortEval.Application.Core.Common.Calculators;

/// <inheritdoc />
public class InstrumentPerformanceCalculator : IInstrumentPerformanceCalculator
{
    /// <inheritdoc />
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