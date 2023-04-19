using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

[SwaggerSchema("Represents an investment portfolio.")]
public class PortfolioDto
{
    [SwaggerSchema("Portfolio identifier.", ReadOnly = true)]
    public int Id { get; set; }

    [SwaggerSchema("Portfolio name.")] public string Name { get; set; }

    [SwaggerSchema("Portfolio note.")] public string Note { get; set; }

    [SwaggerSchema("Portfolio currency code.")]
    public string CurrencyCode { get; set; }
}