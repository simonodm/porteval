using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Application.Services.Interfaces;
using PortEval.Domain.Exceptions;

namespace PortEval.Application.Controllers
{
    [Route("api/import")]
    [ApiController]
    public class CsvImportController : ControllerBase
    {
        private readonly ICsvImportService _importService;

        public CsvImportController(ICsvImportService importService)
        {
            _importService = importService;
        }

        [HttpPost]
        public async Task<ActionResult<CsvTemplateImportResultDto>> UploadFile([FromQuery] IFormFile file, [FromQuery] CsvTemplateType type)
        {
            var result = new CsvTemplateImportResultDto
            {
                ImportId = Guid.NewGuid()
            };

            await using var stream = file.OpenReadStream();
            await _importService.ProcessUpload(result.ImportId, stream, type);

            return Ok(result);
        }

        [HttpGet("log/{id}")]
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
    }
}
