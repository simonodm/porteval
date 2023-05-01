using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.YFinance.Models;

/// <summary>
///     Represents a generic JSON-serializable Yahoo Finance API response.
/// </summary>
/// <typeparam name="TResult">Type of inner response.</typeparam>
public class YahooFinanceResponse<TResult>
{
    [JsonProperty("result")]
    public TResult Result { get; set; }

    [JsonProperty("error")]
    public YahooFinanceError Error { get; set; }
}

/// <summary>
///     Represents a JSON-serializable Yahoo Finance error response.
/// </summary>
public class YahooFinanceError
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }
}