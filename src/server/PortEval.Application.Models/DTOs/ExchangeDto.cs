using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

[SwaggerSchema("Represents a stock exchange.")]
public class ExchangeDto
{
    [SwaggerSchema("The ticker of the stock exchange.")]
    public string Symbol { get; set; }

    [SwaggerSchema("The name of the stock exchange.")]
    public string Name { get; set; }
}