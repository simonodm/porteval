using Newtonsoft.Json;
using System;

namespace PortEval.FinancialDataFetcher.APIs.Tiingo.Models
{
    internal class TiingoPriceResponseModel
    {
        [JsonProperty("date", Required = Required.Always)]
        public DateTime Time { get; set; }
        [JsonProperty("open", Required = Required.Always)]
        public decimal Price { get; set; }
        [JsonProperty("splitFactor")]
        public decimal SplitFactor { get; set; }
    }
}
