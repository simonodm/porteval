using Newtonsoft.Json;

namespace PortEval.FinancialDataFetcher.APIs.Tiingo.Models
{
    internal class TiingoLatestPriceResponseModel
    {
        [JsonProperty("last", Required = Required.Always)]
        public decimal Price { get; set; }
    }
}
