using PortEval.Domain.Exceptions;

namespace PortEval.Domain.Models.ValueObjects
{
    public class DashboardPosition : ValueObject
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public DashboardPosition(int x, int y, int width, int height)
        {
            if (x < 0 || y < 0)
            {
                throw new OperationNotAllowedException("Chart dashboard item coordinates cannot be less than zero.");
            }
            if (x >= 6 || (x + width - 1) >= 6)
            {
                throw new OperationNotAllowedException("Chart dashboard width must fit within 6 columns.");
            }
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool OverlapsWith(DashboardPosition other)
        {
            if (other == null) return false;

            return OverlapsWith(other.X, other.Y, other.Width, other.Height);
        }

        public bool OverlapsWith(int x, int y, int width, int height)
        {
            return X < (x + width) &&
                   (X + Width) > x &&
                   Y < (y + height) &&
                   (Y + Height) > y;
        }
    }
}
