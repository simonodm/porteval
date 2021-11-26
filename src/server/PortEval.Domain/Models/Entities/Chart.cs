using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PortEval.Domain.Models.Entities
{
    public class Chart : VersionedEntity, IAggregateRoot
    {
        public int Id { get; protected set; }
        public string Name { get; private set; }
        public ChartTypeSettings TypeConfiguration { get; private set; }
        public ChartDateRange DateRange { get; private set; }

        public IReadOnlyCollection<ChartLine> Lines
        {
            get => _lines.AsReadOnly();
            protected set => _lines = value.ToList();
        }
        private List<ChartLine> _lines = new List<ChartLine>();

        public Chart(string name, ChartDateRange dateRange, ChartTypeSettings typeConfig)
        {
            Name = name;
            DateRange = dateRange;
            TypeConfiguration = typeConfig;
        }

        public Chart(string name)
        {
            Name = name;
            DateRange = new ChartDateRange(ToDateRange.OneYear);
            TypeConfiguration = ChartTypeSettings.PerformanceChart();
        }

        public ChartLine FindChartLineById(int id)
        {
            var line = _lines.FirstOrDefault(chartLine => chartLine.Id == id);
            if (line == null)
            {
                throw new ItemNotFoundException($"Chart {Id} does not contain line {id}.");
            }

            return line;
        }

        public ChartLine AddChartLine(ChartLine line)
        {
            _lines.Add(line);
            return line;
        }

        public ChartLine AddPortfolioChartLine(int portfolioId, Color color, int width = 1,
            LineDashType dashType = LineDashType.Solid)
        {
            var chartLine = new ChartLinePortfolio(Id, width, dashType, color, portfolioId);
            _lines.Add(chartLine);
            return chartLine;
        }

        public ChartLine AddInstrumentChartLine(int instrumentId, Color color, int width = 1,
            LineDashType dashType = LineDashType.Solid)
        {
            var chartLine = new ChartLineInstrument(Id, width, dashType, color, instrumentId);
            _lines.Add(chartLine);
            return chartLine;
        }

        public ChartLine AddPositionChartLine(int positionId, Color color, int width = 1,
            LineDashType dashType = LineDashType.Solid)
        {
            var chartLine = new ChartLinePosition(Id, width, dashType, color, positionId);
            _lines.Add(chartLine);
            return chartLine;
        }

        public void ReplaceLines(IEnumerable<ChartLine> lines)
        {
            _lines.Clear();
            foreach (var line in lines)
            {
                _lines.Add(line);
            }
        }

        public void RemoveLine(int lineId)
        {
            var line = FindChartLineById(lineId);
            _lines.Remove(line);
        }

        public void Rename(string name)
        {
            Name = name;
        }

        public void SetConfiguration(ChartTypeSettings config)
        {
            TypeConfiguration = config;
        }

        public void SetDateRange(ChartDateRange dateRange)
        {
            DateRange = dateRange;
        }
    }
}
