using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Application.Services.Interfaces;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly IDashboardLayoutQueries _dashboardQueries;
        private readonly ILogger _logger;


        public DashboardController(IDashboardLayoutQueries dashboardQueries, ILoggerFactory loggerFactory, IDashboardService dashboardService)
        {
            _dashboardQueries = dashboardQueries;
            _dashboardService = dashboardService;
            _logger = loggerFactory.CreateLogger(typeof(DashboardController));
        }

        [HttpGet]
        public async Task<ActionResult<DashboardLayoutDto>> GetDashboardLayout()
        {
            _logger.LogInformation("Requesting dashboard configuration.");

            var dashboardLayout = await _dashboardQueries.GetDashboardLayout();

            return dashboardLayout.Response;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDashboardLayout([FromBody] DashboardLayoutDto layout)
        {
            _logger.LogInformation("Updating dashboard layout.");

            await _dashboardService.UpdateDashboardLayout(layout.Items);

            return Ok();
        }
    }
}
