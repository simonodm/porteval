using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortEval.Application.Core.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Controllers;

[Route("imports")]
[ApiController]
public class CsvImportController : PortEvalControllerBase
{
    private readonly ICsvImportService _importService;

    public CsvImportController(ICsvImportService importService)
    {
        _importService = importService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CsvTemplateImportDto>>> GetAllImports()
    {
        var result = await _importService.GetAllImportsAsync();

        return GenerateActionResult(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CsvTemplateImportDto>> GetImport(string id)
    {
        if (!Guid.TryParse(id, out var guid)) return BadRequest("Invalid import log id provided.");

        var result = await _importService.GetImportAsync(guid);
        return GenerateActionResult(result);
    }

    [HttpGet("{id}/log")]
    public IActionResult GetImportErrorLog(string id)
    {
        if (!Guid.TryParse(id, out var guid)) return BadRequest("Invalid import log id provided.");

        var fileStream = _importService.TryGetErrorLog(guid);
        return GenerateFileActionResult(fileStream, "text/csv", "error-log.csv");
    }

    [HttpGet("template")]
    public IActionResult GetImportTemplate(TemplateType templateType)
    {
        var stream = _importService.GetCsvTemplate(templateType);
        return GenerateFileActionResult(stream, "text/csv", $"import_{templateType.ToString().ToLower()}");
    }

    [HttpPost]
    public async Task<ActionResult<CsvTemplateImportDto>> UploadFile([FromForm] IFormFile file,
        [FromForm] TemplateType type)
    {
        await using var stream = file.OpenReadStream();
        var importEntry = await _importService.StartImportAsync(stream, type);
        return GenerateActionResult(importEntry, nameof(GetImport), new { id = importEntry.Response.ImportId });
    }
}