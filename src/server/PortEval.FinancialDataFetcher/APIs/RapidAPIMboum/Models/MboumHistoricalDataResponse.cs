using Newtonsoft.Json;
using System.Collections.Generic;

namespace PortEval.FinancialDataFetcher.APIs.RapidAPIMboum.Models
{

    internal class MboumHistoricalDataResponse
    {
        [JsonProperty("items")]
        public Dictionary<long, MboumHistoricalDataItem> Items { get; set; }

        [JsonProperty("events")]
        public MboumEvents Events { get; set; }

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

    internal class MboumEvents
    {
        [JsonProperty("splits")]
        public Dictionary<long, MboumSplit> Splits { get; set; }
    }

    internal class MboumSplit
    {
        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("numerator")]
        public int Numerator { get; set; }

        [JsonProperty("denominator")]
        public int Denominator { get; set; }
    }
}
