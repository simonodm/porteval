using PortEval.Domain.Models.Enums;

namespace PortEval.Domain.Models.ValueObjects
{
    public class ToDateRange : ValueObject
    {
        public DateRangeUnit Unit { get; }
        public int Value { get; }

        public ToDateRange(DateRangeUnit unit, int value)
        {
            Unit = unit;
            Value = value;
        }
    }
}
