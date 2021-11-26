using Newtonsoft.Json;

namespace PortEval.FinancialDataFetcher.APIs.AlphaVantage.Models
{
    internal class TimeSeriesPriceDataModel
    {
        [JsonProperty("1. open", Required = Required.Always)]
        public decimal Price { get; set; }
    }
}
