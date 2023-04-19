using PortEval.Domain.Models.ValueObjects;

namespace PortEval.Domain.Models.Entities;

/// <summary>
///     A base class for items displayed on the application dashboard.
/// </summary>
public abstract class DashboardItem : Entity, IAggregateRoot
{
    /// <summary>
    ///     ID of the dashboard item.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    ///     Item dimensions and dashboard position.
    /// </summary>
    public DashboardPosition Position { get; private set; }

    /// <summary>
    ///     Creates a 1x1 dashboard item at (0, 0) position.
    /// </summary>
    protected DashboardItem()
    {
        Position = new DashboardPosition(0, 0, 1, 1);
    }

    /// <summary>
    ///     Creates a dashboard item with the dimensions and dashboard position specified.
    /// </summary>
    /// <param name="position">Item dimensions and dashboard position.</param>
    protected DashboardItem(DashboardPosition position)
    {
        Position = position;
    }

    /// <summary>
    ///     Creates a dashboard item with the dimensions and dashboard position specified.
    /// </summary>
    /// <param name="id">ID of the dashboard item.</param>
    /// <param name="position">Item dimensions and dashboard position.</param>
    protected DashboardItem(int id, DashboardPosition position)
    {
        Id = id;
        Position = position;
    }

    /// <summary>
    ///     Changes the dimensions and the position of the dashboard item.
    /// </summary>
    /// <param name="position">Item dimensions and dashboard position.</param>
    public void SetPosition(DashboardPosition position)
    {
        Position = position;
    }
}