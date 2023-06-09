﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.ExchangeRateHost.Models;

internal class ExchangeRatesTimeSeriesResponseModel
{
    [JsonProperty("base", Required = Required.Always)]
    public string Base { get; set; }

    [JsonProperty("rates", Required = Required.Always)]
    public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }
}