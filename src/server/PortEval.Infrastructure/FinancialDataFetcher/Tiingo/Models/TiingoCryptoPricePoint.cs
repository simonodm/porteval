using System;
using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.Tiingo.Models;

public class TiingoCryptoPricePoint
{
    [JsonProperty("date", Required = Required.Always)]
    public DateTime Time { get; set; }
    
    [JsonProperty("open", Required = Required.Always)]
    public decimal Price { get; set; }
}