using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Domain.Models.Entities
{
    public abstract class DashboardItem
    {
        public int Id { get; private set; }
        public DashboardPosition Position { get; private set; }

        public DashboardItem()
        {
            Position = new DashboardPosition(0, 0, 1, 1);
        }

        public DashboardItem(DashboardPosition position)
        {
            Position = position;
        }

        public void SetPosition(DashboardPosition position)
        {
            Position = position;
        }
    }
}
