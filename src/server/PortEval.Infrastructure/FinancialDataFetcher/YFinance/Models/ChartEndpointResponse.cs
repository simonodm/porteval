using System.Collections.Generic;
using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.YFinance.Models;

/// <summary>
///     Represents a JSON-serializable response from Yahoo Finance V8 /chart endpoint.
/// </summary>
public class ChartEndpointResponse
{
    [JsonProperty("chart")]
    public YahooFinanceResponse<List<ChartEndpointResult>> Chart { get; set; }
}

public class ChartEndpointResult
{
    [JsonProperty("meta")]
    public TickerMeta Meta { get; set; }

    [JsonProperty("timestamp")]
    public List<long> Timestamps { get; set; }

    [JsonProperty("indicators")]
    public ChartIndicators Indicators { get; set; }

    [JsonProperty("events")]
    public ChartEndpointEvents Events { get; set; }
}

public class ChartEndpointEvents
{
    [JsonProperty("splits")]
    public Dictionary<string, ChartEndpointSplit> Splits { get; set; }
}

public class ChartEndpointSplit
{
    [JsonProperty("date")]
    public long Timestamp { get; set; }

    [JsonProperty("numerator")]
    public decimal Numerator { get; set; }

    [JsonProperty("denominator")]
    public decimal Denominator { get; set; }
}

public class TickerMeta
{
    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }
}

public class ChartIndicators
{
    [JsonProperty("quote")]
    public List<QuoteIndicator> QuoteIndicators { get; set; }
}

public class QuoteIndicator
{
    [JsonProperty("open")]
    public List<decimal?> Prices { get; set; } // nullable because YFinance sometimes returns nulls in price histories
}