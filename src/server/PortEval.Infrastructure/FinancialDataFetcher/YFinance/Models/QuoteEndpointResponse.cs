using System.Collections.Generic;
using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.YFinance.Models;

/// <summary>
///     Represents a JSON-serializable response from Yahoo Finance V7 /quote endpoint.
/// </summary>
public class QuoteEndpointResponse
{
    [JsonProperty("quoteResponse")]
    public YahooFinanceResponse<List<Quote>> QuoteSummary { get; set; }
}

public class Quote
{
    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("regularMarketPrice")]
    public decimal Price { get; set; }
}