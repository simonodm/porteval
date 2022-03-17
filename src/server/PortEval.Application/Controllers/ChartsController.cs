using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Services.Queries;
using PortEval.Application.Services.Queries.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PortEval.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly IChartService _chartService;
        private readonly IChartQueries _chartQueries;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ChartsController(IChartService chartService, IChartQueries chartQueries, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _chartService = chartService;
            _chartQueries = chartQueries;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger(typeof(ChartsController));
        }


        // GET: api/<ChartsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChartDto>>> GetAllCharts()
        {
            _logger.LogInformation("All charts requested.");

            var charts = await _chartQueries.GetCharts();
            return charts.Response.ToList();
        }

        // GET api/<ChartsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ChartDto>> GetChart(int id)
        {
            _logger.LogInformation($"Chart {id} requested.");

            var chart = await _chartQueries.GetChart(id);
            if (chart.Status == QueryStatus.NotFound)
            {
                return NotFound($"Chart {id} not found.");
            }

            return chart.Response;
        }

        // POST api/<ChartsController>
        [HttpPost]
        public async Task<ActionResult<ChartDto>> PostChart([FromBody] ChartDto createRequest)
        {
            _logger.LogInformation("Creating chart.");

            var createdChart = await _chartService.CreateChartAsync(createRequest);
            return CreatedAtAction("GetChart", new { id = createdChart.Id },
                _mapper.Map<ChartDto>(createdChart));
        }

        // PUT api/<ChartsController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ChartDto>> PutChart(int id, [FromBody] ChartDto updateRequest)
        {
            _logger.LogInformation($"Updating chart {id}.");

            if (id != updateRequest.Id)
            {
                return BadRequest("URL chart id and request body chart id don't match.");
            }

            var updatedChart = await _chartService.UpdateChartAsync(updateRequest);
            return _mapper.Map<ChartDto>(updatedChart);
        }

        // DELETE api/<ChartsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChart(int id)
        {
            _logger.LogInformation($"Deleting chart {id}.");

            await _chartService.DeleteChartAsync(id);
            return Ok();
        }
    }
}
