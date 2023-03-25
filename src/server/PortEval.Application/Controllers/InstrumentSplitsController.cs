using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Features.Interfaces.Queries;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Features.Queries;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("instruments/{instrumentId}/splits")]
    [ApiController]
    public class InstrumentSplitsController : ControllerBase
    {
        private readonly IInstrumentQueries _instrumentQueries;
        private readonly IInstrumentSplitService _splitService;

        public InstrumentSplitsController(IInstrumentQueries instrumentQueries, IInstrumentSplitService splitService)
        {
            _instrumentQueries = instrumentQueries;
            _splitService = splitService;
        }

        // GET instruments/5/splits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstrumentSplitDto>>> GetInstrumentSplits(int instrumentId)
        {
            var result = await _instrumentQueries.GetInstrumentSplits(instrumentId);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Instrument {instrumentId} does not exist.");
            }

            return result.Response.ToList();
        }

        // GET instruments/5/splits/1
        [HttpGet("{splitId}")]
        public async Task<ActionResult<InstrumentSplitDto>> GetInstrumentSplit(int instrumentId, int splitId)
        {
            var result = await _instrumentQueries.GetInstrumentSplit(instrumentId, splitId);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Split {splitId} not found on instrument {instrumentId}.");
            }

            return result.Response;
        }

        // POST instruments/5/splits
        [HttpPost]
        public async Task<IActionResult> PostInstrumentSplit(int instrumentId, [FromBody] InstrumentSplitDto splitData)
        {
            if (instrumentId != splitData.InstrumentId)
            {
                return BadRequest($"Query parameter {nameof(instrumentId)} and body instrument ID don't match.");
            }

            var createdSplit = await _splitService.CreateSplitAsync(splitData);

            return CreatedAtAction(nameof(GetInstrumentSplit), new { instrumentId, splitId = createdSplit.Id });
        }

        // PUT instruments/5/splits/3
        [HttpPut("{splitId}")]
        public async Task<IActionResult> PutInstrumentSplit(int instrumentId, [FromBody] InstrumentSplitDto splitData)
        {
            if (instrumentId != splitData.InstrumentId)
            {
                return BadRequest($"Query parameter {nameof(instrumentId)} and body instrument ID don't match.");
            }

            await _splitService.UpdateSplitAsync(instrumentId, splitData);
            return Ok();
        }
    }
}
