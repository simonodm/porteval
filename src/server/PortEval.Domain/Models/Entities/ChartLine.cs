using System.Drawing;
using PortEval.Domain.Models.Enums;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     A base class for chart lines.
/// </summary>
public abstract class ChartLine : Entity
{
    /// <summary>
    ///     ID of the line.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Line width in pixels.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    ///     Line dash type.
    /// </summary>
    public LineDashType Dash { get; private set; }

    /// <summary>
    ///     ID of the parent chart.
    /// </summary>
    public int ChartId { get; private set; }

    /// <summary>
    ///     Reference to the parent chart.
    /// </summary>
    public Chart Chart { get; }

    /// <summary>
    ///     Line color.
    /// </summary>
    public Color Color { get; private set; }

    /// <summary>
    ///     Initializes a chart line according to the provided configuration.
    /// </summary>
    /// <param name="id">ID of the chart line.</param>
    /// <param name="chartId">ID of the parent chart.</param>
    /// <param name="width">Chart line width in pixels.</param>
    /// <param name="dash">Chart line dash type.</param>
    /// <param name="color">Chart line color.</param>
    protected ChartLine(int id, int chartId, int width, LineDashType dash, Color color) : this(chartId, width, dash,
        color)
    {
        Id = id;
    }

    /// <summary>
    ///     Initializes a chart line according to the provided configuration.
    /// </summary>
    /// <param name="chartId">ID of the parent chart.</param>
    /// <param name="width">Chart line width in pixels.</param>
    /// <param name="dash">Chart line dash type.</param>
    /// <param name="color">Chart line color.</param>
    protected ChartLine(int chartId, int width, LineDashType dash, Color color)
    {
        ChartId = chartId;
        Width = width;
        Dash = dash;
        Color = color;
    }
}