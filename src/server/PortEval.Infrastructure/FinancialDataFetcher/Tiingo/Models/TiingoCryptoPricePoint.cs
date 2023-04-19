using System;
using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.Tiingo.Models;

/// <summary>
///     Represents a JSON-serializable response from Tiingo's <c>/iex</c> endpoint.
/// </summary>
public class TiingoCryptoPricePoint
{
    [JsonProperty("date", Required = Required.Always)]
    public DateTime Time { get; set; }

    [JsonProperty("open", Required = Required.Always)]
    public decimal Price { get; set; }
}