using System.Collections.Generic;
using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.Tiingo.Models;

internal class TiingoCryptoPriceResponseModel
{
    [JsonProperty("ticker")]
    public string Ticker { get; set; }

    [JsonProperty("baseCurrency")]
    public string BaseCurrency { get; set; }

    [JsonProperty("quoteCurrency")]
    public string QuoteCurrency { get; set; }

    [JsonProperty("priceData")]
    public List<TiingoCryptoPricePoint> Data { get; set; }
}