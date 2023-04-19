using System;
using PortEval.Domain.Models.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

[SwaggerSchema("Represents an instrument's split.")]
public class InstrumentSplitDto
{
    [SwaggerSchema("ID of the split.")] public int Id { get; set; }

    [SwaggerSchema("ID of the parent instrument.")]
    public int InstrumentId { get; set; }

    [SwaggerSchema("Time at which the split occurred.")]
    public DateTime Time { get; set; }

    [SwaggerSchema("Denominator of the split factor, e. g. 3-for-1 split's denominator would be 1.")]
    public int SplitRatioDenominator { get; set; }

    [SwaggerSchema("Numerator of the split factor, e. g. 3-for-1 split's numerator would be 3.")]
    public int SplitRatioNumerator { get; set; }

    [SwaggerSchema("Processing status of the split.")]
    public InstrumentSplitProcessingStatus Status { get; set; }
}