using PortEval.Domain.Exceptions;

namespace PortEval.Domain.Models.ValueObjects;

/// <summary>
///     Represents the dimensions and the position of an item on the application dashboard.
/// </summary>
public class DashboardPosition : ValueObject
{
    /// <summary>
    ///     Left offset of the dashboard item.
    /// </summary>
    public int X { get; }

    /// <summary>
    ///     Top offset of the dashboard item.
    /// </summary>
    public int Y { get; }

    /// <summary>
    ///     Width of the dashboard item.
    /// </summary>
    public int Width { get; }

    /// <summary>
    ///     Height of the dashboard item.
    /// </summary>
    public int Height { get; }

    /// <summary>
    ///     Initializes a dashboard position configuration according to the specified parameters.
    /// </summary>
    /// <param name="x">Left offset.</param>
    /// <param name="y">Top offset.</param>
    /// <param name="width">Width of the item.</param>
    /// <param name="height">Height of the item.</param>
    /// <exception cref="OperationNotAllowedException">Thrown if the provided dimensions and/or position are invalid.</exception>
    public DashboardPosition(int x, int y, int width, int height)
    {
        if (x < 0 || y < 0)
        {
            throw new OperationNotAllowedException("Chart dashboard item coordinates cannot be less than zero.");
        }

        if (width <= 0 || height <= 0)
        {
            throw new OperationNotAllowedException("Chart dashboard width and height must be above zero.");
        }

        if (x >= 6 || x + width - 1 >= 6)
        {
            throw new OperationNotAllowedException("Chart dashboard width must fit within 6 columns.");
        }

        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}