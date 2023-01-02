﻿using PortEval.FinancialDataFetcher.Interfaces;
using System;

namespace PortEval.FinancialDataFetcher.Requests
{
    /// <summary>
    /// Request for historical daily prices of the specified cryptocurrency.
    /// </summary>
    public class HistoricalDailyCryptoPricesRequest : InstrumentDataRequest, ITimeRangeRequest
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
