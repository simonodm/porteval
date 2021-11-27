using System;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs
{
    [SwaggerSchema("Represents an entity's performance over a given time period.")]
    public class EntityPerformanceDto
    {
        [SwaggerSchema("Performance itself. -1 is equal to -100%, 1 is equal to 100%", ReadOnly = true)]
        public decimal Performance { get; set; }

        [SwaggerSchema("Time period start.", ReadOnly = true)]
        public DateTime From { get; set; }

        [SwaggerSchema("Time period end.", ReadOnly = true)]
        public DateTime To { get; set; }
    }
}
