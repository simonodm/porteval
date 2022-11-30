using Newtonsoft.Json;
using System.Collections.Generic;

namespace PortEval.FinancialDataFetcher.APIs.RapidAPIMboum.Models
{

    internal class MboumHistoricalDataResponse
    {
        [JsonProperty("items")]
        public Dictionary<long, MboumHistoricalDataItem> Items { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }

    internal class MboumHistoricalDataItem
    {
        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("date_utc")]
        public long DateUtc { get; set; }

        [JsonProperty("open")]
        public decimal Open { get; set; }

        [JsonProperty("close")]
        public decimal Close { get; set; }
    }
}
