using System;

namespace PortEval.Application.Models.DTOs
{
    public class EntityPerformanceDto
    {
        public decimal Performance { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
