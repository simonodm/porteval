using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs
{
    [SwaggerSchema("Represents the dashboard layout.")]
    public class DashboardLayoutDto
    {
        [SwaggerSchema("List of items to display on the dashboard.")]
        public List<DashboardItemDto> Items { get; set; }
    }
}
