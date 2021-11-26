using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PortEval.FinancialDataFetcher.APIs.AlphaVantage.Models
{
    internal class GlobalQuoteResponseModel
    {
        [JsonProperty("Global Quote", Required = Required.Always)]
        public GlobalQuotePriceDataModel PriceData { get; set; }
    }
}
