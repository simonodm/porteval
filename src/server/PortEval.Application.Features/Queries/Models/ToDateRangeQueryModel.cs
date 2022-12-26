using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Features.Queries.Models
{
    internal class ToDateRangeQueryModel
    {
        public DateRangeUnit ToDateRangeUnit { get; set; }
        public int ToDateRangeValue { get; set; }
    }
}
