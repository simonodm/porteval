using System.Collections.Generic;
using Newtonsoft.Json;

namespace PortEval.FinancialDataFetcher.APIs.ExchangeRateHost.Models
{
    internal class ExchangeRatesLatestResponseModel
    {
        [JsonProperty("base")]
        public string Base { get; set; }
        [JsonProperty("rates", Required = Required.Always)]
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
