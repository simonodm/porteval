namespace PortEval.Application.Middleware.Exceptions;

/// <summary>
///     Represents an error API response.
/// </summary>
public class Error
{
    /// <summary>
    ///     HTTP status code of the response.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    ///     Response error message.
    /// </summary>
    public string ErrorMessage { get; }

    /// <summary>
    ///     Initializes the error response with the specified HTTP status code and message.
    /// </summary>
    /// <param name="statusCode">HTTP status code.</param>
    /// <param name="errorMessage">Error message.</param>
    public Error(int statusCode, string errorMessage)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
}