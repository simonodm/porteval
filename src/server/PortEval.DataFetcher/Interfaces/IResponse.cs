using PortEval.DataFetcher.Models;

namespace PortEval.DataFetcher.Interfaces;

/// <summary>
///     Represents a data source response..
/// </summary>
public interface IResponse
{
    /// <summary>
    ///     Status code of the response.
    /// </summary>
    public StatusCode StatusCode { get; set; }

    /// <summary>
    ///     Response error message if applicable.
    /// </summary>
    public string ErrorMessage { get; set; }
}