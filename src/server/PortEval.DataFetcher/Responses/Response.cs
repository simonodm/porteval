using PortEval.DataFetcher.Interfaces;
using PortEval.DataFetcher.Models;

namespace PortEval.DataFetcher.Responses;

/// <summary>
///     Represents an operation's response. Contains operation's status code, error message and retrieved data if the
///     operation has been successful.
/// </summary>
/// <typeparam name="TResult">Retrieved data type</typeparam>
public class Response<TResult> : IResponse
{
    /// <summary>
    ///     Result of the operation if there is one.
    /// </summary>
    public TResult Result { get; set; }

    /// <summary>
    ///     Status code representing whether the operation was successful.
    /// </summary>
    public StatusCode StatusCode { get; set; }

    /// <summary>
    ///     Error message if applicable.
    /// </summary>
    public string ErrorMessage { get; set; }
}