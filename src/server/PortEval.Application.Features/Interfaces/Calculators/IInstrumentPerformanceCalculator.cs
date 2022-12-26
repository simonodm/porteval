namespace PortEval.Application.Features.Interfaces.Calculators
{
    /// <summary>
    /// Calculates the performance of the instrument.
    /// </summary>
    public interface IInstrumentPerformanceCalculator
    {
        /// <summary>
        /// Calculates the performance of the instrument in a specific range based on its prices at the start and at the end of the range.
        /// </summary>
        /// <param name="priceAtStart">Price at the start of the range.</param>
        /// <param name="priceAtEnd">Price at the end of the range.</param>
        /// <returns>Instrument's performance.</returns>
        public decimal CalculatePerformance(decimal priceAtStart, decimal priceAtEnd);
    }
}
