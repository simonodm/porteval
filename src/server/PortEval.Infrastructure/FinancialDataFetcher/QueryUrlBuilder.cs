using System.Text;

namespace PortEval.Infrastructure.FinancialDataFetcher;

/// <summary>
///     A URL builder allowing addition of query parameters.
/// </summary>
public class QueryUrlBuilder
{
    private readonly StringBuilder _urlBuilder = new();
    private bool _queryParamsExist;

    /// <summary>
    ///     Initializes the builder with the specified URL.
    /// </summary>
    /// <param name="baseUrl">The base url to use during parameter building.</param>
    public QueryUrlBuilder(string baseUrl)
    {
        _urlBuilder.Append(baseUrl);
    }

    /// <summary>
    ///     Adds a query parameter to the URL with the specified key and value.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    public void AddQueryParam(string name, string value)
    {
        if (name == null || value == null) return;

        _urlBuilder.Append(_queryParamsExist ? '&' : '?');
        _urlBuilder.Append($"{name}={value}");

        _queryParamsExist = true;
    }

    /// <summary>
    ///     Converts the builder to a URL string containing all added query parameters.
    /// </summary>
    /// <returns>A URL string.</returns>
    public override string ToString()
    {
        return _urlBuilder.ToString();
    }
}