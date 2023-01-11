using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.Tiingo.Models
{
    internal class TiingoIexTopPriceResponseModel
    {
        [JsonProperty("last", Required = Required.Always)]
        public decimal Price { get; set; }
    }
}
