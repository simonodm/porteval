using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("instruments/{instrumentId}/splits")]
    [ApiController]
    public class InstrumentSplitsController : PortEvalControllerBase
    {
        private readonly IInstrumentSplitService _splitService;

        public InstrumentSplitsController(IInstrumentSplitService splitService)
        {
            _splitService = splitService;
        }

        // GET instruments/5/splits
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstrumentSplitDto>>> GetInstrumentSplits(int instrumentId)
        {
            var result = await _splitService.GetInstrumentSplitsAsync(instrumentId);
            return GenerateActionResult(result);
        }

        // GET instruments/5/splits/1
        [HttpGet("{splitId}")]
        public async Task<ActionResult<InstrumentSplitDto>> GetInstrumentSplit(int instrumentId, int splitId)
        {
            var result = await _splitService.GetInstrumentSplitAsync(instrumentId, splitId);
            return GenerateActionResult(result);
        }

        // POST instruments/5/splits
        [HttpPost]
        public async Task<ActionResult<InstrumentSplitDto>> PostInstrumentSplit(int instrumentId, [FromBody] InstrumentSplitDto splitData)
        {
            if (instrumentId != splitData.InstrumentId)
            {
                return BadRequest($"Query parameter {nameof(instrumentId)} and body instrument ID don't match.");
            }

            var createdSplit = await _splitService.CreateSplitAsync(splitData);
            return GenerateActionResult(createdSplit, nameof(GetInstrumentSplit), new { instrumentId, splitId = createdSplit.Response.Id });
        }

        // PUT instruments/5/splits/3
        [HttpPut("{splitId}")]
        public async Task<ActionResult<InstrumentSplitDto>> PutInstrumentSplit(int instrumentId, [FromBody] InstrumentSplitDto splitData)
        {
            if (instrumentId != splitData.InstrumentId)
            {
                return BadRequest($"Query parameter {nameof(instrumentId)} and body instrument ID don't match.");
            }

            var updatedSplit = await _splitService.UpdateSplitAsync(instrumentId, splitData);
            return GenerateActionResult(updatedSplit);
        }
    }
}
