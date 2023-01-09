using System.Net.Http;

namespace PortEval.DataFetcher
{
    public abstract class DataSource
    {
        protected DataSourceConfiguration Configuration { get; set; }
        protected HttpClient HttpClient { get; set; }
    }
}
