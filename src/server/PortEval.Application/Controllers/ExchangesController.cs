using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Controllers;

[Route("exchanges")]
[ApiController]
public class ExchangesController : PortEvalControllerBase
{
    private readonly IInstrumentService _instrumentService;

    public ExchangesController(IInstrumentService instrumentService)
    {
        _instrumentService = instrumentService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExchangeDto>>> GetKnownExchanges()
    {
        var result = await _instrumentService.GetKnownExchangesAsync();
        return GenerateActionResult(result);
    }
}