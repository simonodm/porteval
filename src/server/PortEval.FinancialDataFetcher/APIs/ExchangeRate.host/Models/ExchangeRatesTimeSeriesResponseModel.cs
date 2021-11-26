﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace PortEval.FinancialDataFetcher.APIs.ExchangeRate.host.Models
{
    internal class ExchangeRatesTimeSeriesResponseModel
    {
        [JsonProperty("base", Required = Required.Always)]
        public string Base { get; set; }

        [JsonProperty("rates", Required = Required.Always)]
        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }
    }
}
