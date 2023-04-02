using System.Net.Http;

namespace PortEval.DataFetcher
{
    /// <summary>
    /// A base class for external data sources which can be registered in the library.
    /// </summary>
    public abstract class DataSource
    {
        /// <summary>
        /// Contains configuration of the data source, such as its credentials.
        /// </summary>
        protected DataSourceConfiguration Configuration { get; set; }

        /// <summary>
        /// An HTTP client to use for HTTP operations.
        /// </summary>
        protected HttpClient HttpClient { get; set; }
    }
}
