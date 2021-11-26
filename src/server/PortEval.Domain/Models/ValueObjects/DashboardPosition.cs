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
            if (x >= 6 || (x + width - 1) >= 6)
            {
                throw new OperationNotAllowedException($"Chart dashboard width must fit within 6 columns.");
            }
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool OverlapsWith(DashboardPosition other)
        {
            if (other == null) return false;

            return X < (other.X + other.Width) && (X + Width) > other.X && Y < (other.Y + other.Height) &&
                   (Y + Height) > other.Y;
        }
    }
}
