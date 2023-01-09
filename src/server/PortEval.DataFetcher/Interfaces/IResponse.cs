using PortEval.DataFetcher.Models;

namespace PortEval.DataFetcher.Interfaces
{
    /// <summary>
    /// Represents an operation's response.
    /// </summary>
    public interface IResponse
    {
        public StatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
