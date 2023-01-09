using PortEval.DataFetcher.Interfaces;
using PortEval.DataFetcher.Models;

namespace PortEval.DataFetcher.Responses
{
    /// <summary>
    /// Represents an operation's response. Contains operation's status code, error message and retrieved data if the operation has been successful.
    /// </summary>
    /// <typeparam name="TResult">Retrieved data type</typeparam>
    public class Response<TResult> : IResponse
    {
        public StatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public TResult Result { get; set; }
    }
}
