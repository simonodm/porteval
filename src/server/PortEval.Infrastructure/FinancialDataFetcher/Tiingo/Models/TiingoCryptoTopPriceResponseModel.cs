using System.Collections.Generic;
using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.Tiingo.Models;

internal class TiingoCryptoTopPriceResponseModel
{
    [JsonProperty("quoteCurrency")]
    public string QuoteCurrency { get; set; }

    [JsonProperty("ticker")]
    public string Ticker { get; set; }

    [JsonProperty("baseCurrency")]
    public string BaseCurrency { get; set; }

    [JsonProperty("topOfBookData")]
    public List<TiingoCryptoTopPriceDataResponseModel> Data { get; set; }
}

internal class TiingoCryptoTopPriceDataResponseModel
{
    [JsonProperty("lastPrice")]
    public decimal LastPrice { get; set; }

    [JsonProperty("askPrice")]
    public decimal AskPrice { get; set; }

    [JsonProperty("bidPrice")]
    public decimal BidPrice { get; set; }
}