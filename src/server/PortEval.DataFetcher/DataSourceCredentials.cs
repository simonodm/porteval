namespace PortEval.DataFetcher;

/// <summary>
///     Represents credentials to the data source.
/// </summary>
public class DataSourceCredentials
{
    /// <summary>
    ///     Contains the username to use to access the data source.
    ///     This property can also be used for client IDs.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    ///     Contains the password to use to access the data source.
    ///     This property can also be used for client secrets.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    ///     Contains a generic token to use to access the data source.
    ///     This property can also be used for refresh tokens.
    /// </summary>
    public string Token { get; set; }
}