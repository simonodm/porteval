using Newtonsoft.Json.Linq;
using PortEval.FinancialDataFetcher.Models;
using PortEval.FinancialDataFetcher.Responses;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PortEval.FinancialDataFetcher.APIs
{
    internal static class HttpClientExtensions
    {
        /// <summary>
        /// Fetches JSON from the specified endpoint and processes it according to the supplied callback. Takes rate limiting into account.
        /// </summary>
        /// <param name="httpClient">HttpClient to use for the fetch</param>
        /// <param name="queryUrl">URL to fetch</param>
        /// <param name="rateLimiter">Request rate limiter</param>
        /// <typeparam name="TResult">Target response type after parsing.</typeparam>
        /// <returns>A Response object containing the operation status and parsed data if the operation was successful.</returns>
        public static async Task<Response<TResult>> FetchJson<TResult>(this HttpClient httpClient, string queryUrl, RateLimiter rateLimiter = null)
        {
            try
            {
                if (rateLimiter != null && !rateLimiter.AllowRequest())
                {
                    return new Response<TResult>
                    {
                        StatusCode = StatusCode.ConnectionError,
                        ErrorMessage = "Request limit exceeded."
                    };
                }

                var responseToken = JToken.Parse(await httpClient.GetStringAsync(queryUrl));
                var response = responseToken.ToObject<TResult>();
                var isValidResponse = response != null;
                return new Response<TResult>
                {
                    StatusCode = isValidResponse ? StatusCode.Ok : StatusCode.OtherError,
                    ErrorMessage = isValidResponse ? "" : "Invalid data received.",
                    Result = response
                };
            }
            catch (Exception ex)
            {
                return new Response<TResult>
                {
                    StatusCode = ex is HttpRequestException httpEx && httpEx.StatusCode != HttpStatusCode.NotFound ? StatusCode.ConnectionError : StatusCode.OtherError,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
