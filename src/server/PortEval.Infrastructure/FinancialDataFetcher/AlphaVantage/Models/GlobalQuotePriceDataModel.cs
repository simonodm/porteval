using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.AlphaVantage.Models;

internal class GlobalQuotePriceDataModel
{
    [JsonProperty("05. price", Required = Required.Always)]
    public decimal Price { get; set; }
}