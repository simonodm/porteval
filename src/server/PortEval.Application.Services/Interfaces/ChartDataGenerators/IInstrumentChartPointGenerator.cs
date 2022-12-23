using PortEval.Application.Features.Common;
using System;

namespace PortEval.Application.Features.Interfaces.ChartDataGenerators
{
    public interface IInstrumentChartPointGenerator : IDisposable
    {
        public InstrumentPriceChartPointData GetNextChartPointData();
        public bool IsFinished();
    }
}
