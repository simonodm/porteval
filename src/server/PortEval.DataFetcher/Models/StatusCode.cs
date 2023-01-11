namespace PortEval.DataFetcher.Models
{
    /// <summary>
    /// Represents status of an operation. 
    /// </summary>
    public enum StatusCode
    {
        /// <summary>
        /// If the operation was successful and requested data were retrieved.
        /// </summary>
        Ok,

        /// <summary>
        /// If the operation failed due to connectivity problems.
        /// </summary>
        ConnectionError,

        /// <summary>
        /// If the operation failed for a reason not related to connection.
        /// </summary>
        OtherError,

        /// <summary>
        /// If the operation failed due to rate limit being exceeded.
        /// </summary>
        RateLimitExceeded
    }
}
