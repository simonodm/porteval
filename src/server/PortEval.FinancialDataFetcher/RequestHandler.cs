using PortEval.FinancialDataFetcher.APIs.Interfaces;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortEval.FinancialDataFetcher
{
    /// <summary>
    /// Wraps requests' processing logic, including handling retries and request distribution among supported APIs.
    /// </summary>
    /// <typeparam name="TClient">API client type</typeparam>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResponse">Response type</typeparam>
    internal class RequestHandler<TClient, TRequest, TResponse>
        where TClient : class, IFinancialApiClient<TRequest, TResponse>
        where TRequest : Request
        where TResponse : IResponse, new()
    {
        private readonly TRequest _request;
        private readonly List<TClient> _eligibleApis;
        private readonly TClient _priorityApi;
        private readonly RetryPolicy _retryPolicy;

        public RequestHandler(TRequest request, List<TClient> eligibleApis)
        {
            _request = request;

            if (eligibleApis.Count == 0)
            {
                throw new ArgumentException("No eligible API clients passed to request handler.");
            }
            _eligibleApis = eligibleApis;

#if DEBUG
            _retryPolicy = RetryPolicy.Fast;
#else
            _retryPolicy = RetryPolicy.Standard;
#endif
        }

        public RequestHandler(TRequest request, List<TClient> eligibleApis, TClient priorityApi) : this(request, eligibleApis)
        {
            _priorityApi = priorityApi;
        }

        /// <summary>
        /// Processes the request.
        /// </summary>
        /// <returns>A Response object containing processing status and retrieved data if the request is successful.</returns>
        public async Task<TResponse> Handle()
        {
            var priorityApi = GetPriorityApi();
            var result = await priorityApi.Process(_request);
            if (result.StatusCode == StatusCode.Ok)
            {
                return result;
            }

            return await TryAllEligibleApis();
        }

        /// <summary>
        /// Gets the API client to prioritize for the request from the handler's list of supported APIs.
        /// </summary>
        /// <returns>Priority API client</returns>
        private TClient GetPriorityApi()
        {
            return _priorityApi ?? _eligibleApis[0]; // TODO: design better algorithm for priority selection
        }

        /// <summary>
        /// Distributes the request among all supported APIs with the retry policy specified in the constructor. When any of the API clients succeeds,
        /// all the remaining requests get cancelled and the data from the successful API gets returned. 
        /// </summary>
        /// <returns>A Response object containing processing status and retrieved data if the request is successful.</returns>
        private async Task<TResponse> TryAllEligibleApis()
        {
            var apiTasks = new List<Task<TResponse>>();

            using (var cts = new CancellationTokenSource())
            {
                foreach (var api in _eligibleApis)
                {
                    var retryableJob = new RetryableAsyncJob<TResponse>(async () => await api.Process(_request), _retryPolicy);
                    apiTasks.Add(RunRetryableJob(retryableJob, cts));
                }

                await Task.WhenAll(apiTasks.ToArray<Task>());
            }

            foreach (var task in apiTasks)
            {
                if (task.Result.StatusCode == StatusCode.Ok)
                {
                    return task.Result;
                }
            }

            return new TResponse
            {
                StatusCode = StatusCode.OtherError,
                ErrorMessage = "No API was able to process the request correctly."
            };
        }

        /// <summary>
        /// Initiates a retryable fetch job with the retry policy specified in the constructor.
        /// </summary>
        /// <param name="retryableJob">Job to retry</param>
        /// <param name="cts">Cancellation token source</param>
        /// <returns>A Response object containing processing status and retrieved data if the request was successful.</returns>
        private Task<TResponse> RunRetryableJob(RetryableAsyncJob<TResponse> retryableJob, CancellationTokenSource cts)
        {
            return Task.Run(async () =>
            {
                try
                {
                    while (retryableJob.CanRetry())
                    {
                        var jobResult = await retryableJob.Retry(cts.Token);
                        if (jobResult.StatusCode == StatusCode.Ok)
                        {
                            cts.Cancel();
                            return jobResult;
                        }

                        if (jobResult.StatusCode != StatusCode.ConnectionError)
                        {
                            return jobResult;
                        }
                    }

                    return new TResponse
                    {
                        StatusCode = StatusCode.ConnectionError,
                        ErrorMessage = "Cannot access the API."
                    };
                }
                catch (TaskCanceledException)
                {
                    return new TResponse
                    {
                        StatusCode = StatusCode.OtherError,
                        ErrorMessage = "Operation cancelled."
                    };
                }
            });
        }
    }
}
