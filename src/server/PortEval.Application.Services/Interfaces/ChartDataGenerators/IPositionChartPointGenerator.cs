using System;
using PortEval.Application.Features.Common;

namespace PortEval.Application.Features.Interfaces.ChartDataGenerators
{
    public interface IPositionChartPointGenerator : IDisposable
    {
        public PositionChartPointData GetNextChartPointData();
        public bool IsFinished();
    }
}
