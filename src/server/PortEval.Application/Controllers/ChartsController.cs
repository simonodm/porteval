using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PortEval.Application.Controllers
{
    [Route("charts")]
    [ApiController]
    public class ChartsController : PortEvalControllerBase
    {
        private readonly IChartService _chartService;

        public ChartsController(IChartService chartService)
        {
            _chartService = chartService;
        }
        
        // GET: api/<ChartsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChartDto>>> GetAllCharts()
        {
            var charts = await _chartService.GetAllChartsAsync();
            return GenerateActionResult(charts);
        }

        // GET api/<ChartsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChartDto>> GetChart(int id)
        {
            var chart = await _chartService.GetChartAsync(id);
            return GenerateActionResult(chart);
        }

        // POST api/<ChartsController>
        [HttpPost]
        public async Task<ActionResult<ChartDto>> PostChart([FromBody] ChartDto createRequest)
        {
            var createdChart = await _chartService.CreateChartAsync(createRequest);
            return GenerateActionResult(createdChart, nameof(GetChart), new { id = createdChart.Response.Id });
        }

        // PUT api/<ChartsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ChartDto>> PutChart(int id, [FromBody] ChartDto updateRequest)
        {
            if (id != updateRequest.Id)
            {
                return BadRequest("URL chart id and request body chart id don't match.");
            }

            var updatedChart = await _chartService.UpdateChartAsync(updateRequest);
            return GenerateActionResult(updatedChart);
        }

        // DELETE api/<ChartsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChart(int id)
        {
            var response = await _chartService.DeleteChartAsync(id);
            return GenerateActionResult(response);
        }
    }
}
