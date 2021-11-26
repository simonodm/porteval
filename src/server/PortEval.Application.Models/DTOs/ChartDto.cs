using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Models.DTOs
{
    public class ChartDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateRangeStart { get; set; }
        public DateTime? DateRangeEnd { get; set; }
        public bool? IsToDate { get; set; }
        public ToDateRange? ToDateRange { get; set; }
        public ChartType Type { get; set; }
        public string CurrencyCode { get; set; }
        public AggregationFrequency? Frequency { get; set; }
        public List<ChartLineDto> Lines { get; set; }
    }
}
