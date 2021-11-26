using Newtonsoft.Json;
using System.Collections.Generic;

namespace PortEval.FinancialDataFetcher.APIs.AlphaVantage.Models
{
    internal class TimeSeriesIntradayResponseModel
    {
        [JsonProperty("Time Series (60min)", Required = Required.DisallowNull)]
        public Dictionary<string, TimeSeriesPriceDataModel> Prices { get; set; }

        // Since there are two possible property names in the response for time series (depending on the interval supplied as part of the query),
        //   this workaround allows us to map them both to a single deserialized object's property.
        [JsonProperty("Time Series (5min)")]
        private Dictionary<string, TimeSeriesPriceDataModel> PricesFiveMin
        {
            set => Prices = value;
        }
    }
}
