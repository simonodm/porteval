using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("[controller]")]
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
