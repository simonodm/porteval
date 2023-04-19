using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents a dashboard item containing a chart.
/// </summary>
public class DashboardChartItem : DashboardItem
{
    /// <summary>
    ///     ID of the chart displayed in this dashboard item.
    /// </summary>
    public int ChartId { get; private set; }

    internal DashboardChartItem(int chartId)
    {
        ChartId = chartId;
    }

    internal DashboardChartItem(int chartId, DashboardPosition position) : base(position)
    {
        ChartId = chartId;
    }

    internal DashboardChartItem(int id, int chartId, DashboardPosition position) : base(id, position)
    {
        ChartId = chartId;
    }

    /// <summary>
    ///     A factory method for creating a chart dashboard item.
    /// </summary>
    /// <param name="chart">A reference to the displayed chart.</param>
    /// <param name="position">The item's position on the dashboard.</param>
    /// <returns>The newly created dashboard chart item entity.</returns>
    public static DashboardChartItem Create(Chart chart, DashboardPosition position)
    {
        return new DashboardChartItem(chart.Id, position);
    }
}