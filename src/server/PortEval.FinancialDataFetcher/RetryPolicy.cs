using System;
using System.Collections.Generic;

namespace PortEval.FinancialDataFetcher
{
    /// <summary>
    /// Represents retry count and retry intervals.
    /// </summary>
    internal class RetryPolicy
    {
        /// <summary>
        /// Represents the standard retry policy with 1min, 2min, 3min, 5min, 10min, 20min retry intervals.
        /// </summary>
        public static RetryPolicy Standard
        {
            get
            {
                var policy = new RetryPolicy();
                policy.RetryIntervals.Add(TimeSpan.FromMinutes(1));
                policy.RetryIntervals.Add(TimeSpan.FromMinutes(2));
                policy.RetryIntervals.Add(TimeSpan.FromMinutes(3));
                policy.RetryIntervals.Add(TimeSpan.FromMinutes(5));
                policy.RetryIntervals.Add(TimeSpan.FromMinutes(10));
                policy.RetryIntervals.Add(TimeSpan.FromMinutes(20));
                return policy;
            }
        }

        /// <summary>
        /// Represents the fast retry policy with three retries in five-second intervals.
        /// </summary>
        public static RetryPolicy Fast
        {
            get
            {
                var policy = new RetryPolicy();
                policy.RetryIntervals.Add(TimeSpan.FromSeconds(5));
                policy.RetryIntervals.Add(TimeSpan.FromSeconds(5));
                policy.RetryIntervals.Add(TimeSpan.FromSeconds(5));
                return policy;
            }
        }

        public List<TimeSpan> RetryIntervals { get; } = new List<TimeSpan>();
    }
}
