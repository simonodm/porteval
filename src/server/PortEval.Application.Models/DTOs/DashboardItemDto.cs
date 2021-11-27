using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs
{
    [SwaggerSchema("Represents a single chart on the application's dashboard.")]
    public class DashboardItemDto
    {
        [SwaggerSchema("X position of the item. Values between 0 and 5 are allowed.")]
        public int DashboardPositionX { get; set; }

        [SwaggerSchema("Y position of the item.")]
        public int DashboardPositionY { get; set; }

        [SwaggerSchema("Item width. Values between 1 and 6 are allowed.")]
        public int DashboardWidth { get; set; }

        [SwaggerSchema("Item height.")]
        public int DashboardHeight { get; set; }

        [SwaggerSchema("Identifier of the chart to display.")]
        public int ChartId { get; set; }
    }
}
