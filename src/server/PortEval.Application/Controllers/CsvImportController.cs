using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Services.Interfaces;
using PortEval.Application.Services.Queries;
using PortEval.Application.Services.Queries.Interfaces;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Controllers
{
    [Route("api/imports")]
    [ApiController]
    public class CsvImportController : ControllerBase
    {
        private readonly ICsvImportService _importService;
        private readonly IDataImportQueries _importQueries;
        private readonly ILogger _logger;

        public CsvImportController(ICsvImportService importService, IDataImportQueries importQueries, ILoggerFactory loggerFactory)
        {
            _importService = importService;
            _importQueries = importQueries;
            _logger = loggerFactory.CreateLogger(typeof(CsvImportController));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CsvTemplateImportDto>>> GetAllImports()
        {
            _logger.LogInformation("Imports requested.");

            var result = await _importQueries.GetAllImports();

            return result.Response.ToList();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CsvTemplateImportDto>> GetImport(string id)
        {
            _logger.LogInformation($"Import {id} requested.");

            if (!Guid.TryParse(id, out Guid guid))
            {
                return BadRequest("Invalid import log id provided.");
            }

            var result = await _importQueries.GetImport(guid);
            if (result.Status == QueryStatus.NotFound)
            {
                return NotFound($"Import {id} not found.");
            }

            return result.Response;
        }

        [HttpGet("{id}/log")]
        public IActionResult GetImportErrorLog(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                return BadRequest("Invalid import log id provided.");
            }

            var fileStream = _importService.TryGetErrorLog(guid);
            if (fileStream == null)
            {
                return BadRequest($"No error log found for import {id}.");
            }

            return File(fileStream, "text/csv", "error-log.csv");
        }

        [HttpGet("template")]
        public IActionResult GetImportTemplate(TemplateType templateType)
        {
            var stream = _importService.GetCsvTemplate(templateType);
            return File(stream, "text/csv", $"import_{templateType.ToString().ToLower()}");
        }

        [HttpPost]
        public async Task<ActionResult<CsvTemplateImportResultDto>> UploadFile([FromForm] IFormFile file, [FromForm] TemplateType type)
        {
            await using var stream = file.OpenReadStream();
            var importEntry = await _importService.StartImport(stream, type);

            return Ok(new CsvTemplateImportResultDto
            {
                ImportId = importEntry.Id
            });
        }
    }
}
