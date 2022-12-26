using PortEval.Application.Features.Common;
using System;

namespace PortEval.Application.Features.Interfaces.ChartDataGenerators
{
    /// <summary>
    /// Iteratively generates the data needed to calculate instrument metrics for multiple consecutive date ranges.
    /// </summary>
    public interface IInstrumentRangeDataGenerator : IDisposable
    {
        /// <summary>
        /// Calculates and returns the data needed to calculate instrument metrics in the next date range.
        ///
        /// Each subsequent call to this method will return the data for the date range following the one returned by the previous call until
        /// there is no more data to generate, in which the <see cref="IsFinished"/> will return <c>true</c>.
        /// </summary>
        /// <returns>A <see cref="InstrumentPriceRangeData"/> instance containing all the data needed to calculate metrics in the next date range.</returns>
        public InstrumentPriceRangeData GetNextRangeData();

        /// <summary>
        /// Determines whether there is any more data to generate by calls to <see cref="GetNextRangeData"/>.
        /// </summary>
        /// <returns><c>true</c> if the generator is done and there is no more data to return, <c>false</c> otherwise.</returns>
        public bool IsFinished();
    }
}
