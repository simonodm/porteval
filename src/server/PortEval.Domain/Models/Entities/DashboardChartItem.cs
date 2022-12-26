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

        public static DashboardChartItem Create(int chartId)
        {
            return new DashboardChartItem(chartId);
        }

        public static DashboardChartItem Create(int chartId, DashboardPosition position)
        {
            return new DashboardChartItem(chartId, position);
        }
    }
}
