using PortEval.DataFetcher.Responses;
using System.Threading.Tasks;

namespace PortEval.DataFetcher.Interfaces
{
    /// <summary>
    /// Aggregates multiple API clients supporting retrieval of homogenous data.
    /// </summary>
    public interface IDataFetcher
    {
        /// <summary>
        /// Processes the provided request and returns its response.
        /// </summary>
        /// <param name="request">Request to process</param>
        /// <typeparam name="TRequest">Request type</typeparam>
        /// <typeparam name="TResult">Response data type</typeparam>
        /// <returns></returns>
        public Task<Response<TResult>> ProcessRequest<TRequest, TResult>(TRequest request)
            where TRequest : class, IRequest;
    }
}
