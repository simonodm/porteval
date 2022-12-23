using System;
using PortEval.Application.Features.Common;

namespace PortEval.Application.Features.Interfaces.ChartDataGenerators
{
    /// <summary>
    /// Iteratively generates the data needed to calculate the next chart point of a position chart line.
    ///
    /// Instances of this class are stateful and should only be used once.
    /// </summary>
    public interface IPositionChartPointGenerator : IDisposable
    {
        /// <summary>
        /// Calculates and returns the data needed to calculate the next chart point of a position chart line.
        ///
        /// Each subsequent call to this method will return the data for the chart point following the one returned by the previous call until
        /// there is no more data to generate, in which the <see cref="IsFinished"/> will return <c>true</c>.
        /// </summary>
        /// <returns>A <see cref="PositionChartPointData"/> instance containing all the data needed for the next chart point.</returns>
        public PositionChartPointData GetNextChartPointData();

        /// <summary>
        /// Determines whether there is any more data to generate by calls to <see cref="GetNextChartPointData"/>.
        /// </summary>
        /// <returns><c>true</c> if the generator is done and there is no more data to return, <c>false</c> otherwise.</returns>
        public bool IsFinished();
    }
}
