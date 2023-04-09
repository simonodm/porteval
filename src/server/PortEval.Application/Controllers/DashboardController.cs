using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("dashboard")]
    [ApiController]
    public class DashboardController : PortEvalControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardLayoutDto>> GetDashboardLayout()
        {
            var dashboardLayout = await _dashboardService.GetDashboardLayoutAsync();
            return GenerateActionResult(dashboardLayout);
        }

        [HttpPost]
        public async Task<ActionResult<DashboardLayoutDto>> UpdateDashboardLayout([FromBody] DashboardLayoutDto layout)
        {
            var updatedLayout = await _dashboardService.UpdateDashboardLayoutAsync(layout.Items);
            return GenerateActionResult(updatedLayout);
        }
    }
}
