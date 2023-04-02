using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Core.Interfaces.Queries;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Core.Queries;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PortEval.Application.Controllers
{
    [Route("charts")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly IChartService _chartService;
        private readonly IChartQueries _chartQueries;

        public ChartsController(IChartService chartService, IChartQueries chartQueries)
        {
            _chartService = chartService;
            _chartQueries = chartQueries;
        }


        // GET: api/<ChartsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChartDto>>> GetAllCharts()
        {
            var charts = await _chartQueries.GetCharts();
            return charts.Response.ToList();
        }

        // GET api/<ChartsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChartDto>> GetChart(int id)
        {
            var chart = await _chartQueries.GetChart(id);
            if (chart.Status == QueryStatus.NotFound)
            {
                return NotFound($"Chart {id} not found.");
            }

            return chart.Response;
        }

        // POST api/<ChartsController>
        [HttpPost]
        public async Task<IActionResult> PostChart([FromBody] ChartDto createRequest)
        {
            var createdChart = await _chartService.CreateChartAsync(createRequest);

            return CreatedAtAction(nameof(GetChart), new { id = createdChart.Id }, null);
        }

        // PUT api/<ChartsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChart(int id, [FromBody] ChartDto updateRequest)
        {
            if (id != updateRequest.Id)
            {
                return BadRequest("URL chart id and request body chart id don't match.");
            }

            await _chartService.UpdateChartAsync(updateRequest);
            return Ok();
        }

        // DELETE api/<ChartsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChart(int id)
        {
            await _chartService.DeleteChartAsync(id);
            return Ok();
        }
    }
}
