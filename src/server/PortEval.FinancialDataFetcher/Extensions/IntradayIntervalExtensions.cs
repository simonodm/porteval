using PortEval.FinancialDataFetcher.Models;
using System;

namespace PortEval.FinancialDataFetcher.Extensions
{
    internal static class IntradayIntervalExtensions
    {
        public static TimeSpan ToTimeSpan(this IntradayInterval interval)
        {
            switch (interval)
            {
                case IntradayInterval.FiveMinutes:
                    return TimeSpan.FromMinutes(5);
                case IntradayInterval.OneHour:
                    return TimeSpan.FromHours(1);
                default:
                    throw new Exception($"Unrecognized intraday interval supplied: {interval}.");
            }
        }
    }
}
