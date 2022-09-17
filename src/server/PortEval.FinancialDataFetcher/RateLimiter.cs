using System;

namespace PortEval.FinancialDataFetcher
{
    /// <summary>
    /// Limits request rate based on the supplied window size and request per window limit. Drops requests over the specified limits.
    /// </summary>
    public class RateLimiter
    {
        private readonly object _requestLock = new object();

        private readonly TimeSpan _windowSize;
        private readonly int _requestsPerWindow;
        private int _currentWindowRequests;
        private DateTime _firstWindowRequest;

        public RateLimiter(TimeSpan windowSize, int requestsPerWindow)
        {
            _windowSize = windowSize;
            _requestsPerWindow = requestsPerWindow;
        }

        /// <summary>
        /// Checks whether a request can be allowed through based on configured parameters.
        /// </summary>
        /// <returns>true if the request can be allowed through, false otherwise.</returns>
        public bool AllowRequest()
        {
            lock (_requestLock)
            {
                var now = DateTime.UtcNow;
                if (now - _windowSize >= _firstWindowRequest)
                {
                    _currentWindowRequests = 1;
                    _firstWindowRequest = now;
                    return true;
                }
                if (_currentWindowRequests < _requestsPerWindow)
                {
                    _currentWindowRequests++;
                    return true;
                }

                return false;
            }
        }
    }
}
