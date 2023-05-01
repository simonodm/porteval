using System;
using PortEval.Domain.Models.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

public class CsvTemplateImportDto
{
    [SwaggerSchema("Guid of the import.")]
    public Guid ImportId { get; set; }

    [SwaggerSchema("Time of import.")]
    public DateTime Time { get; set; }

    [SwaggerSchema("Type of data which were imported.")]
    public TemplateType TemplateType { get; set; }

    [SwaggerSchema("Specifies whether the error log is available for download.")]
    public bool ErrorLogAvailable { get; set; }

    [SwaggerSchema("Specifies the error log URL.")]
    public string ErrorLogUrl { get; set; }

    [SwaggerSchema("Specifies the import status.")]
    public ImportStatus Status { get; set; }

    [SwaggerSchema("Specifies import status message if there is one.")]
    public string StatusDetails { get; set; }
}