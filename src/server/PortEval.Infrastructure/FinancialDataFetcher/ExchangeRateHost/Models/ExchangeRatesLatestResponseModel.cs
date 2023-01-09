using Newtonsoft.Json;
using System.Collections.Generic;

namespace PortEval.Infrastructure.FinancialDataFetcher.ExchangeRateHost.Models
{
    internal class ExchangeRatesLatestResponseModel
    {
        [JsonProperty("base")]
        public string Base { get; set; }
        [JsonProperty("rates", Required = Required.Always)]
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
