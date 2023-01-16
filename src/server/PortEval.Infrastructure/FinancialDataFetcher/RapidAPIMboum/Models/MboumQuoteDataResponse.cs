﻿using Newtonsoft.Json;

namespace PortEval.Infrastructure.FinancialDataFetcher.RapidAPIMboum.Models
{
    public class MboumQuoteDataResponse
    {
        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("exchange")]
        public string Exchange { get; set; }

        [JsonProperty("regularMarketPrice")]
        public decimal RegularMarketPrice { get; set; }
    }
}