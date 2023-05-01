using System.Collections.Generic;
using System.Linq;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents a user-defined line chart.
/// </summary>
public class Chart : VersionedEntity, IAggregateRoot
{
    /// <summary>
    ///     ID of the chart.
    /// </summary>
    public int Id { get; protected set; }

    /// <summary>
    ///     User-defined chart name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Chart type.
    /// </summary>
    public ChartTypeSettings TypeConfiguration { get; private set; }

    /// <summary>
    ///     Chart date range.
    /// </summary>
    public ChartDateRange DateRange { get; private set; }

    /// <summary>
    ///     A read-only collection of chart lines.
    /// </summary>
    public IReadOnlyCollection<ChartLine> Lines
    {
        get => _lines.AsReadOnly();
        protected set => _lines = value.ToList();
    }

    private List<ChartLine> _lines = new();

    internal Chart(int id, string name, ChartDateRange dateRange, ChartTypeSettings typeConfig) : this(name, dateRange,
        typeConfig)
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
        DateRange = new ChartDateRange(new ToDateRange(DateRangeUnit.Year, 1));
        TypeConfiguration = ChartTypeSettings.PerformanceChart();
    }

    /// <summary>
    ///     A factory method creating a chart based on the specified parameters.
    /// </summary>
    /// <param name="name">Chart name.</param>
    /// <param name="dateRange">Chart date range.</param>
    /// <param name="typeConfig">Chart type configuration.</param>
    /// <returns>The newly created chart domain entity.</returns>
    public static Chart Create(string name, ChartDateRange dateRange, ChartTypeSettings typeConfig)
    {
        return new Chart(name, dateRange, typeConfig);
    }

    /// <summary>
    ///     A factory method for creating a 1Y price chart with the specified name.
    /// </summary>
    /// <param name="name">Chart name.</param>
    /// <returns>The newly created chart domain entity.</returns>
    public static Chart Create(string name)
    {
        return new Chart(name);
    }

    /// <summary>
    ///     Replaces the lines of the chart.
    /// </summary>
    /// <param name="lines">New chart lines.</param>
    /// <exception cref="OperationNotAllowedException">
    ///     Thrown if there are multiple chart lines representing the same financial
    ///     entity.
    /// </exception>
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
                throw new OperationNotAllowedException(
                    $"Instrument with ID {instrumentLine.InstrumentId} is already added to the chart.");
            }

            if (line is ChartLinePosition positionLine && !positionLineIds.Add(positionLine.PositionId))
            {
                throw new OperationNotAllowedException(
                    $"Position with ID {positionLine.PositionId} is already added to the chart.");
            }

            if (line is ChartLinePortfolio portfolioLine && !portfolioLineIds.Add(portfolioLine.PortfolioId))
            {
                throw new OperationNotAllowedException(
                    $"Portfolio with ID {portfolioLine.PortfolioId} is already added to the chart.");
            }

            newLines.Add(line);
        }

        // Replacing lines at this point is safe, because we have already checked for duplicates
        _lines = newLines;
    }

    /// <summary>
    ///     Renames the chart.
    /// </summary>
    /// <param name="name">New chart name.</param>
    public void Rename(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Changes the type of the chart.
    /// </summary>
    /// <param name="config">New chart type configuration.</param>
    public void SetConfiguration(ChartTypeSettings config)
    {
        TypeConfiguration = config;
    }

    /// <summary>
    ///     Changes the date range of the chart.
    /// </summary>
    /// <param name="dateRange">New chart date range.</param>
    public void SetDateRange(ChartDateRange dateRange)
    {
        DateRange = dateRange;
    }
}