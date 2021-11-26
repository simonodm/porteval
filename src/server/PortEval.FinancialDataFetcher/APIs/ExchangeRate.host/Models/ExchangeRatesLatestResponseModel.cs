using Newtonsoft.Json;
using System.Collections.Generic;

namespace PortEval.FinancialDataFetcher.APIs.ExchangeRate.host.Models
{
    internal class ExchangeRatesLatestResponseModel
    {
        [JsonProperty("base")]
        public string Base { get; set; }
        [JsonProperty("rates", Required = Required.Always)]
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
