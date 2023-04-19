using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.AlphaVantage.Models;

internal class GlobalQuoteResponseModel
{
    [JsonProperty("Global Quote", Required = Required.Always)]
    public GlobalQuotePriceDataModel PriceData { get; set; }
}