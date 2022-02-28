using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Queries.Models
{
    internal class ToDateRangeQueryModel
    {
        public DateRangeUnit ToDateRangeUnit { get; set; }
        public int ToDateRangeValue { get; set; }
    }
}
