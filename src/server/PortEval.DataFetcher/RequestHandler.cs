using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PortEval.DataFetcher.Interfaces;
using PortEval.DataFetcher.Models;
using PortEval.DataFetcher.Responses;

namespace PortEval.DataFetcher;

/// <summary>
///     Wraps requests' processing logic, including handling retries and request distribution among supported APIs.
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResult">Result type</typeparam>
internal class RequestHandler<TRequest, TResult>
    where TRequest : class, IRequest
{
    private readonly List<DataSource> _eligibleApis;
    private readonly DataSource _priorityApi;
    private readonly TRequest _request;
    private readonly RetryPolicy _retryPolicy;

    public RequestHandler(TRequest request, List<DataSource> eligibleApis, RetryPolicy retryPolicy = null)
    {
        _request = request;

        if (eligibleApis.Count == 0)
        {
            throw new ArgumentException("No eligible API clients passed to request handler.");
        }

        _eligibleApis = eligibleApis;

        if (retryPolicy != null)
        {
            _retryPolicy = retryPolicy;
        }
        else
        {
#if DEBUG
            _retryPolicy = RetryPolicy.Fast;
#else
            _retryPolicy = RetryPolicy.Standard;
#endif
        }
    }

    public RequestHandler(TRequest request, List<DataSource> eligibleApis, DataSource priorityApi) : this(request,
        eligibleApis)
    {
        _priorityApi = priorityApi;
    }

    /// <summary>
    ///     Processes the request.
    /// </summary>
    /// <returns>A Response object containing processing status and retrieved data if the request is successful.</returns>
    public async Task<Response<TResult>> HandleAsync()
    {
        var priorityApi = GetPriorityApi();
        var result = await ProcessUsingDataSourceAsync(priorityApi);
        if (result.StatusCode == StatusCode.Ok)
        {
            return result;
        }

        return await TryAllEligibleApisAsync();
    }

    /// <summary>
    ///     Gets the API client to prioritize for the request from the handler's list of supported APIs.
    /// </summary>
    /// <returns>Priority API client</returns>
    private DataSource GetPriorityApi()
    {
        return _priorityApi ?? _eligibleApis[0];
    }

    /// <summary>
    ///     Distributes the request among all supported APIs with the retry policy specified in the constructor. When any of
    ///     the API clients succeeds,
    ///     all the remaining requests get cancelled and the data from the successful API gets returned.
    /// </summary>
    /// <returns>A Response object containing processing status and retrieved data if the request is successful.</returns>
    private async Task<Response<TResult>> TryAllEligibleApisAsync()
    {
        var apiTasks = new List<Task<Response<TResult>>>();

        using (var cts = new CancellationTokenSource())
        {
            foreach (var api in _eligibleApis)
            {
                var retryableJob =
                    new RetryableAsyncJob<Response<TResult>>(async () => await ProcessUsingDataSourceAsync(api),
                        _retryPolicy);
                apiTasks.Add(RunRetryableJobAsync(retryableJob, cts));
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

        return new Response<TResult>
        {
            StatusCode = StatusCode.OtherError,
            ErrorMessage = "No API was able to process the request correctly."
        };
    }

    /// <summary>
    ///     Initiates a retryable fetch job with the retry policy specified in the constructor.
    /// </summary>
    /// <param name="retryableJob">Job to retry</param>
    /// <param name="cts">Cancellation token source</param>
    /// <returns>A Response object containing processing status and retrieved data if the request was successful.</returns>
    private Task<Response<TResult>> RunRetryableJobAsync(RetryableAsyncJob<Response<TResult>> retryableJob,
        CancellationTokenSource cts)
    {
        return Task.Run(async () =>
        {
            try
            {
                while (retryableJob.CanRetry())
                {
                    var jobResult = await retryableJob.RetryAsync(cts.Token);
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

                return new Response<TResult>
                {
                    StatusCode = StatusCode.ConnectionError,
                    ErrorMessage = "Cannot access the API."
                };
            }
            catch (TaskCanceledException)
            {
                return new Response<TResult>
                {
                    StatusCode = StatusCode.OtherError,
                    ErrorMessage = "Operation cancelled."
                };
            }
            catch (Exception ex)
            {
                return new Response<TResult>
                {
                    StatusCode = StatusCode.OtherError,
                    ErrorMessage = ex.Message
                };
            }
        });
    }

    private async Task<Response<TResult>> ProcessUsingDataSourceAsync(DataSource dataSource)
    {
        var type = dataSource.GetType();
        var methods = type.GetMethods();
        var methodToCall = methods.FirstOrDefault(m =>
        {
            var attribute = m.GetCustomAttribute<RequestProcessorAttribute>();
            return attribute != null && attribute.CanProcess<TRequest, TResult>();
        });

        if (methodToCall == null)
        {
            return new Response<TResult>
            {
                StatusCode = StatusCode.OtherError,
                ErrorMessage = $"API unable to process request of type {nameof(TRequest)}."
            };
        }

        if (methodToCall.ReturnType.IsAssignableTo(typeof(Task<Response<TResult>>)))
        {
            var result = methodToCall.Invoke(dataSource, new object[] { _request });
            return await (Task<Response<TResult>>)result;
        }

        if (methodToCall.ReturnType.IsAssignableTo(typeof(Response<TResult>)))
        {
            return (Response<TResult>)methodToCall.Invoke(dataSource, new object[] { _request });
        }

        throw new InvalidOperationException(
            $"{methodToCall.Name} has an unrecognized return type {methodToCall.ReturnType}.");
    }
}