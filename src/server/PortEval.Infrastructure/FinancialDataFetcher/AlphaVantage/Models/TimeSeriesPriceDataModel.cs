using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.AlphaVantage.Models;

internal class TimeSeriesPriceDataModel
{
    [JsonProperty("1. open", Required = Required.Always)]
    public decimal Price { get; set; }

    [JsonProperty("8. split coefficient")]
    public decimal SplitCoefficient { get; set; }
}