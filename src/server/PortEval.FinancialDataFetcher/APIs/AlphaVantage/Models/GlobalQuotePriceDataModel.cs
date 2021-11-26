using Newtonsoft.Json;

namespace PortEval.FinancialDataFetcher.APIs.AlphaVantage.Models
{
    internal class GlobalQuotePriceDataModel
    {
        [JsonProperty("05. price", Required = Required.Always)]
        public decimal Price { get; set; }
    }
}
