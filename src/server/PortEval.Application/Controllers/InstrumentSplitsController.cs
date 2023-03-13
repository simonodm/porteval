using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly IMapper _mapper;

        public InstrumentSplitsController(IInstrumentQueries instrumentQueries, IInstrumentSplitService splitService, IMapper mapper)
        {
            _instrumentQueries = instrumentQueries;
            _splitService = splitService;
            _mapper = mapper;
        }

        // GET: instruments/5/splits
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

        // POST instruments/5/splits
        [HttpPost]
        public async Task<ActionResult<InstrumentSplitDto>> PostInstrumentSplit(int instrumentId, [FromBody] InstrumentSplitDto splitData)
        {
            if (instrumentId != splitData.InstrumentId)
            {
                return BadRequest($"Query parameter {nameof(instrumentId)} and body instrument ID don't match.");
            }

            var createdSplit = await _splitService.CreateSplitAsync(splitData);

            return CreatedAtAction(nameof(GetInstrumentSplits), new { instrumentId },
                _mapper.Map<InstrumentSplitDto>(createdSplit));
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

            return _mapper.Map<InstrumentSplitDto>(updatedSplit);
        }
    }
}
