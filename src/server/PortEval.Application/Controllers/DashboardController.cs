using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models.DTOs;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Services;

namespace PortEval.Application.Controllers
{
    [Route("dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly IDashboardLayoutQueries _dashboardQueries;


        public DashboardController(IDashboardLayoutQueries dashboardQueries, IDashboardService dashboardService)
        {
            _dashboardQueries = dashboardQueries;
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardLayoutDto>> GetDashboardLayout()
        {
            var dashboardLayout = await _dashboardQueries.GetDashboardLayout();

            return dashboardLayout.Response;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDashboardLayout([FromBody] DashboardLayoutDto layout)
        {
            await _dashboardService.UpdateDashboardLayout(layout.Items);

            return Ok();
        }
    }
}
