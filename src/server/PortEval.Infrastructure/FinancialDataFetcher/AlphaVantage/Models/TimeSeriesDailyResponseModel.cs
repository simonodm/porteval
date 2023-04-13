using Newtonsoft.Json;
using System.Collections.Generic;

namespace PortEval.Infrastructure.FinancialDataFetcher.AlphaVantage.Models
{
    internal class TimeSeriesDailyResponseModel
    {
        [JsonProperty("Time Series (Daily)", Required = Required.Always)]
        public Dictionary<string, TimeSeriesPriceDataModel> Prices { get; set; }
    }

}
