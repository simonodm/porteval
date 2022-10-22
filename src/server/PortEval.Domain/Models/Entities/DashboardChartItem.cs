using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Domain.Models.Entities
{
    public class DashboardChartItem : DashboardItem
    {
        public int ChartId { get; private set; }

        public DashboardChartItem(int chartId)
        {
            ChartId = chartId;
        }

        public DashboardChartItem(int chartId, DashboardPosition position) : base(position)
        {
            ChartId = chartId;
        }
    }
}
