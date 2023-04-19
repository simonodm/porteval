using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

[SwaggerSchema("Represents a single currency.")]
public class CurrencyDto
{
    [SwaggerSchema("Currency three-letter code.")]
    public string Code { get; set; }

    [SwaggerSchema("Currency name.", ReadOnly = true)]
    public string Name { get; set; }

    [SwaggerSchema("Currency symbol, e. g. US$ for american dollar.", ReadOnly = true)]
    public string Symbol { get; set; }

    [SwaggerSchema("Whether this currency is configured as the application's default currency.")]
    public bool IsDefault { get; set; }
}