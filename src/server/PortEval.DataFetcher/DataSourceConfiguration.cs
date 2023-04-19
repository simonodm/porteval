namespace PortEval.DataFetcher;

/// <summary>
///     Represents configuration of a single data source.
/// </summary>
public class DataSourceConfiguration
{
    /// <summary>
    ///     Contains data source credentials.
    /// </summary>
    public DataSourceCredentials Credentials { get; set; }

    /// <summary>
    ///     Contains the rate limiter to use for throttling requests to the data source.
    /// </summary>
    public RateLimiter RateLimiter { get; set; }
}