using PortEval.FinancialDataFetcher.Requests;
using PortEval.FinancialDataFetcher.Responses;
using System.Threading.Tasks;

namespace PortEval.FinancialDataFetcher.APIs.Interfaces
{
    /// <summary>
    /// Represents an API client.
    /// </summary>
    public interface IFinancialApi { }

    /// <summary>
    /// Represents an API client supporting a given request type.
    /// </summary>
    /// <typeparam name="TRequest">Request type which the client can process</typeparam>
    /// <typeparam name="TResponse">Response object type which the client returns for the given <c>TRequest</c></typeparam>
    public interface IFinancialApi<TRequest, TResponse> : IFinancialApi
        where TRequest : IRequest
        where TResponse : IResponse
    {
        /// <summary>
        /// Fetches and processes the data according to parameters specified in the request.
        /// </summary>
        /// <param name="request">Request to fetch data for</param>
        /// <returns>Response object containing operation status and fetched data if the operation was successful.</returns>
        public Task<TResponse> Process(TRequest request);
    }
}
