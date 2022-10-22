using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortEval.FinancialDataFetcher
{
    /// <summary>
    /// Represents a single retryable async job to retry with the specified retry policy.
    /// </summary>
    /// <typeparam name="TResult">Job result type</typeparam>
    internal class RetryableAsyncJob<TResult>
    {
        private readonly Func<Task<TResult>> _func;
        private readonly RetryPolicy _retryPolicy;
        private int _attempt;
        private DateTime? _lastAttempt;

        public RetryableAsyncJob(Func<Task<TResult>> func, RetryPolicy retryPolicy)
        {
            _func = func;
            _retryPolicy = retryPolicy;
        }

        /// <summary>
        /// Retries the job.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Job result</returns>
        /// <exception cref="ApplicationException">Thrown if no retries are available as per the retry policy.</exception>
        public async Task<TResult> Retry(CancellationToken cancellationToken)
        {
            if (!CanRetry()) throw new ApplicationException("Action failed - no retries left.");

            var currentInterval = _retryPolicy.RetryIntervals[_attempt];

            if (_lastAttempt != null)
            {
                var timeSinceLastAttempt = (TimeSpan)(DateTime.UtcNow - _lastAttempt);
                if (timeSinceLastAttempt < currentInterval)
                {
                    var remainingTime = currentInterval - timeSinceLastAttempt;
                    await Task.Delay((int)remainingTime.TotalMilliseconds, cancellationToken);
                }
            }

            _lastAttempt = DateTime.UtcNow;
            _attempt++;

            return await _func.Invoke();
        }

        /// <summary>
        /// Checks whether the job can be retried according to the specified retry policy.
        /// </summary>
        /// <returns>true if there are retries left, false otherwise.</returns>
        public bool CanRetry()
        {
            return _attempt < _retryPolicy.RetryIntervals.Count;
        }
    }
}
