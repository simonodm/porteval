﻿using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using System.Collections.Generic;
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

        public Chart(int id, string name, ChartDateRange dateRange, ChartTypeSettings typeConfig) : this(name, dateRange, typeConfig)
        {
            Id = id;
        }

        public Chart(string name, ChartDateRange dateRange, ChartTypeSettings typeConfig)
        {
            Name = name;
            DateRange = dateRange;
            TypeConfiguration = typeConfig;
        }

        public Chart(int id, string name) : this(name)
        {
            Id = id;
        }

        public Chart(string name)
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
            _lines.Clear();
            foreach (var line in lines)
            {
                _lines.Add(line);
            }
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
