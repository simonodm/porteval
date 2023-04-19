namespace PortEval.Application.Core.Interfaces.Calculators;

/// <summary>
///     Calculates the profit of the instrument.
/// </summary>
public interface IInstrumentProfitCalculator
{
    /// <summary>
    ///     Calculates the profit of the instrument in a specific range based on its prices at the start and at the end of the
    ///     range.
    /// </summary>
    /// <param name="priceAtStart">Price at the start of the range.</param>
    /// <param name="priceAtEnd">Price at the end of the range.</param>
    /// <returns>Instrument's profit.</returns>
    public decimal CalculateProfit(decimal priceAtStart, decimal priceAtEnd);
}