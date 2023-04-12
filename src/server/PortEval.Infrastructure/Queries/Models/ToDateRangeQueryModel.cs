using PortEval.Domain.Models.Enums;

namespace PortEval.Infrastructure.Queries.Models
{
    public class ToDateRangeQueryModel
    {
        public DateRangeUnit ToDateRangeUnit { get; set; }
        public int ToDateRangeValue { get; set; }
    }
}
