using System.Threading.Tasks;
using PortEval.DataFetcher.Responses;

namespace PortEval.DataFetcher.Interfaces;

/// <summary>
///     Aggregates multiple API clients supporting retrieval of homogenous data.
/// </summary>
public interface IDataFetcher
{
    /// <summary>
    ///     Processes the provided request and returns its response.
    /// </summary>
    /// <param name="request">Request to process</param>
    /// <typeparam name="TRequest">Request type</typeparam>
    /// <typeparam name="TResult">Response data type</typeparam>
    /// <returns>
    ///     A task representing the asynchronous request processing.
    ///     Task result contains a <see cref="Response{T}" /> instance containing the result of the request.
    /// </returns>
    public Task<Response<TResult>> ProcessRequestAsync<TRequest, TResult>(TRequest request)
        where TRequest : class, IRequest;
}