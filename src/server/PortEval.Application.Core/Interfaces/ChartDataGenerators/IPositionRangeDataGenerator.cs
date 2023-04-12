using PortEval.Application.Core.Common;
using System;

namespace PortEval.Application.Core.Interfaces.ChartDataGenerators
{
    /// <summary>
    /// Iteratively generates the data needed to calculate position metrics for multiple consecutive date ranges.
    /// </summary>
    public interface IPositionRangeDataGenerator : IDisposable
    {
        /// <summary>
        /// Calculates and returns the data needed to calculate position metrics in the next date range.
        ///
        /// Each subsequent call to this method will return the data for the date range following the one returned by the previous call until
        /// there is no more data to generate, in which the <see cref="IsFinished"/> will return <c>true</c>.
        /// </summary>
        /// <returns>A <see cref="PositionPriceRangeData"/> instance containing all the data needed to calculate metrics for the next date range.</returns>
        public PositionPriceRangeData GetNextRangeData();

        /// <summary>
        /// Determines whether there is any more data to generate by calls to <see cref="GetNextRangeData"/>.
        /// </summary>
        /// <returns><c>true</c> if the generator is done and there is no more data to return, <c>false</c> otherwise.</returns>
        public bool IsFinished();
    }
}
