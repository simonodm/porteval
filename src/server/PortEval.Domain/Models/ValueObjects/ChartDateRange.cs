using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Enums;
using System;

namespace PortEval.Domain.Models.ValueObjects
{
    public class ChartDateRange : ValueObject
    {
        public DateTime? Start { get; private set; }
        public DateTime? End { get; private set; }
        public bool IsToDate { get; private set; }
        public ToDateRange ToDateRange { get; private set; }

        public ChartDateRange()
        {
            IsToDate = true;
            ToDateRange = new ToDateRange(DateRangeUnit.DAY, 1);
        }

        public ChartDateRange(DateTime start, DateTime end)
        {
            if (end < start)
            {
                throw new OperationNotAllowedException("Chart date range start cannot be later than the end.");
            }
            if ((end - start).TotalDays < 1)
            {
                throw new OperationNotAllowedException("Chart date range cannot be less than 1 day.");
            }

            Start = start;
            End = end;
        }

        public ChartDateRange(ToDateRange toDateRange)
        {
            IsToDate = true;
            ToDateRange = toDateRange;
        }
    }
}
