using PortEval.DataFetcher.Interfaces;
using PortEval.DataFetcher.Models;
using PortEval.DataFetcher.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace PortEval.DataFetcher
{
    /// <inheritdoc />
    public class DataFetcher : IDataFetcher
    {
        private readonly HttpClient _httpClient;
        private readonly List<DataSource> _registeredClients = new List<DataSource>();
        private readonly RetryPolicy _retryPolicy;

        /// <summary>
        /// Initializes the data fetcher.
        /// </summary>
        public DataFetcher()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Initializes the data fetcher with pre-configured <see cref="HttpClient"/>
        /// </summary>
        /// <param name="httpClient">Http client.</param>
        public DataFetcher(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Initializes the data fetcher with pre-configured <see cref="RetryPolicy" />
        /// </summary>
        /// <param name="retryPolicy">Retry policy to use when retrying requests</param>
        public DataFetcher(RetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        /// <summary>
        /// Initializes the data fetcher with pre-configured <see cref="HttpClient"/> and <see cref="RetryPolicy"/>
        /// </summary>
        /// <param name="httpClient">Http client</param>
        /// <param name="retryPolicy">Retry policy to use when retrying requests.</param>
        public DataFetcher(HttpClient httpClient, RetryPolicy retryPolicy)
        {
            _httpClient = httpClient;
            _retryPolicy = retryPolicy;
        }

        /// <summary>
        /// Registers a data source to be used for request processing.
        /// </summary>
        /// <typeparam name="TDataSource">Type of the data source.</typeparam>
        /// <param name="configuration">An optional configuration object.</param>
        public void RegisterDataSource<TDataSource>(DataSourceConfiguration configuration = null)
            where TDataSource : DataSource, new()
        {
            var api = new TDataSource();

            var type = typeof(TDataSource);
            var expectedConfigurationPropertyName = "Configuration";
            var expectedHttpClientPropertyName = "HttpClient";
            var configProp = type.GetProperty(expectedConfigurationPropertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (configProp is null)
            {
                throw new ArgumentException($"No \"{expectedConfigurationPropertyName}\" property found on type {type}.");
            }

            var httpClientProp = type.GetProperty(expectedHttpClientPropertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (httpClientProp is null)
            {
                throw new ArgumentException(
                    $"No \"{expectedHttpClientPropertyName}\" property found on type {type}.");
            }

            configProp.SetValue(api, configuration, null);
            httpClientProp.SetValue(api, _httpClient, null);

            _registeredClients.Add(api);
        }

        /// <inheritdoc />
        public async Task<Response<TResult>> ProcessRequest<TRequest, TResult>(TRequest request)
            where TRequest : class, IRequest
        {
            var eligibleApis = _registeredClients.Where(CanProcessRequest<TRequest, TResult>).ToList();

            if (eligibleApis.Count == 0)
            {
                return new Response<TResult>
                {
                    StatusCode = StatusCode.OtherError,
                    ErrorMessage = "No eligible API found for the given request."
                };
            }

            var handler = new RequestHandler<TRequest, TResult>(request, eligibleApis, _retryPolicy);
            return await handler.Handle();
        }

        private static bool CanProcessRequest<TRequest, TResult>(DataSource source)
            where TRequest : IRequest
        {
            var type = source.GetType();
            var typeMethods = type.GetMethods();
            var requestProcessAttribute = typeMethods
                .Select(m => m.GetCustomAttribute<RequestProcessorAttribute>())
                .FirstOrDefault(attr => attr != null && attr.CanProcess<TRequest, TResult>());

            return requestProcessAttribute != null;
        }
    }
}
