using System.Drawing;
using PortEval.Domain.Models.Enums;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     A chart line representing an instrument.
/// </summary>
public class ChartLineInstrument : ChartLine
{
    /// <summary>
    ///     ID of the instrument represented by this line.
    /// </summary>
    public int InstrumentId { get; }

    internal ChartLineInstrument(int id, int chartId, int width, LineDashType dash, Color color, int instrumentId) :
        base(id, chartId, width, dash, color)
    {
        InstrumentId = instrumentId;
    }

    internal ChartLineInstrument(int chartId, int width, LineDashType dash, Color color, int instrumentId) : base(
        chartId, width, dash, color)
    {
        InstrumentId = instrumentId;
    }

    /// <summary>
    ///     A factory method creating the chart line according to the specified parameters.
    /// </summary>
    /// <param name="chartId">ID of the parent chart.</param>
    /// <param name="width">Chart line width in pixels.</param>
    /// <param name="dash">Chart line dash type.</param>
    /// <param name="color">Chart line color.</param>
    /// <param name="instrument">A reference to the instrument represented by this line.</param>
    /// <returns>The newly created instrument chart line.</returns>
    public static ChartLineInstrument Create(int chartId, int width, LineDashType dash, Color color,
        Instrument instrument)
    {
        return new ChartLineInstrument(chartId, width, dash, color, instrument.Id);
    }
}