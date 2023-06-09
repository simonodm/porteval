﻿using System;
using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.Tiingo.Models;

internal class TiingoPriceResponseModel
{
    [JsonProperty("date", Required = Required.Always)]
    public DateTime Time { get; set; }

    [JsonProperty("adjOpen", Required = Required.Always)]
    public decimal Price { get; set; }
}