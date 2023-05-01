using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.RapidAPIMboum.Models;

/// <summary>
///     Represents a JSON-serializable response of the <c>/market/quotes</c> endpoint provided by RapidAPI Mboum.
/// </summary>
public class MboumQuoteDataResponse
{
    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("exchange")]
    public string Exchange { get; set; }

    [JsonProperty("regularMarketPrice")]
    public decimal RegularMarketPrice { get; set; }
}