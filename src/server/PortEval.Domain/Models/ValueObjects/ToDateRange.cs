using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
