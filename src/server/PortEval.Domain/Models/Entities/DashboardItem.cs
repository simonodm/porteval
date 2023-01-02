using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Domain.Models.Entities
{
    public abstract class DashboardItem : Entity, IAggregateRoot
    {
        public int Id { get; private set; }
        public DashboardPosition Position { get; private set; }

        protected DashboardItem()
        {
            Position = new DashboardPosition(0, 0, 1, 1);
        }

        protected DashboardItem(DashboardPosition position)
        {
            Position = position;
        }

        protected DashboardItem(int id, DashboardPosition position)
        {
            Id = id;
            Position = position;
        }

        public void SetPosition(DashboardPosition position)
        {
            Position = position;
        }
    }
}
