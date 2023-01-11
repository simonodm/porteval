namespace PortEval.DataFetcher
{
    public class DataSourceConfiguration
    {
        public DataSourceCredentials Credentials { get; set; }
        public RateLimiter RateLimiter { get; set; }
    }
}
