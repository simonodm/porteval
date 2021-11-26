using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace PortEval.FinancialDataFetcher.APIs.OpenExchangeRates.Models
{
    internal class LatestExchangeRatesResponseModel
    {
        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("timestamp", Required = Required.Always)]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Time { get; set; }

        [JsonProperty("rates")]
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
