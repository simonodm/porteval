using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Domain.Models.Entities
{
    public class DashboardChartItem : DashboardItem
    {
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

        public static DashboardChartItem Create(Chart chart)
        {
            return new DashboardChartItem(chart.Id);
        }

        public static DashboardChartItem Create(Chart chart, DashboardPosition position)
        {
            return new DashboardChartItem(chart.Id, position);
        }
    }
}
