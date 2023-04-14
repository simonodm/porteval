using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using PortEval.Domain.Exceptions;

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

        internal Chart(int id, string name, ChartDateRange dateRange, ChartTypeSettings typeConfig) : this(name, dateRange, typeConfig)
        {
            Id = id;
        }

        internal Chart(string name, ChartDateRange dateRange, ChartTypeSettings typeConfig)
        {
            Name = name;
            DateRange = dateRange;
            TypeConfiguration = typeConfig;
        }

        internal Chart(int id, string name) : this(name)
        {
            Id = id;
        }

        internal Chart(string name)
        {
            Name = name;
            DateRange = new ChartDateRange(new ToDateRange(DateRangeUnit.YEAR, 1));
            TypeConfiguration = ChartTypeSettings.PerformanceChart();
        }

        public static Chart Create(string name, ChartDateRange dateRange, ChartTypeSettings typeConfig)
        {
            return new Chart(name, dateRange, typeConfig);
        }

        public static Chart Create(string name)
        {
            return new Chart(name);
        }

        public ChartLine FindChartLineById(int id)
        {
            var line = _lines.FirstOrDefault(chartLine => chartLine.Id == id);
            return line;
        }

        public void ReplaceLines(IEnumerable<ChartLine> lines)
        {
            var instrumentLineIds = new HashSet<int>();
            var positionLineIds = new HashSet<int>();
            var portfolioLineIds = new HashSet<int>();

            var newLines = new List<ChartLine>();
            foreach (var line in lines)
            {
                if (line is ChartLineInstrument instrumentLine && !instrumentLineIds.Add(instrumentLine.InstrumentId))
                {
                    throw new OperationNotAllowedException($"Instrument with ID {instrumentLine.InstrumentId} is already added to the chart.");
                }
                if (line is ChartLinePosition positionLine && !positionLineIds.Add(positionLine.PositionId))
                {
                    throw new OperationNotAllowedException($"Position with ID {positionLine.PositionId} is already added to the chart.");
                }
                if (line is ChartLinePortfolio portfolioLine && !portfolioLineIds.Add(portfolioLine.PortfolioId))
                {
                    throw new OperationNotAllowedException($"Portfolio with ID {portfolioLine.PortfolioId} is already added to the chart.");
                }

                newLines.Add(line);
            }

            // Replacing lines at this point is safe, because we have already checked for duplicates
            _lines = newLines;
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
