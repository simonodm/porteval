namespace PortEval.Application.Middleware.Exceptions
{
    /// <summary>
    /// Represents an error response.
    /// </summary>
    public class Error
    {
        public int StatusCode { get; }
        public string ErrorMessage { get; }

        public Error(int statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
