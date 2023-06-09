﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PortEval.DataFetcher;
using PortEval.DataFetcher.Models;
using PortEval.DataFetcher.Responses;

namespace PortEval.Infrastructure.FinancialDataFetcher.Extensions;

internal static class HttpClientExtensions
{
    /// <summary>
    ///     Fetches JSON from the specified endpoint. Takes rate limiting into account.
    /// </summary>
    /// <param name="httpClient">HTTP client to use for the fetch</param>
    /// <param name="queryUrl">URL to fetch</param>
    /// <param name="rateLimiter">Request rate limiter</param>
    /// <param name="headers">Optional HTTP headers to add to the request</param>
    /// <typeparam name="TResult">Target response type after parsing.</typeparam>
    /// <returns>A Response object containing the operation status and parsed data if the operation was successful.</returns>
    public static async Task<Response<TResult>> GetJsonAsync<TResult>(this HttpClient httpClient, string queryUrl,
        RateLimiter rateLimiter = null, Dictionary<string, string> headers = null)
    {
        try
        {
            if (rateLimiter != null && !rateLimiter.AllowRequest())
            {
                return new Response<TResult>
                {
                    StatusCode = StatusCode.RateLimitExceeded,
                    ErrorMessage = "Request limit exceeded."
                };
            }

            using var request = new HttpRequestMessage(HttpMethod.Get, queryUrl);
            if (headers != null)
            {
                foreach (var (header, value) in headers)
                {
                    request.Headers.Add(header, value);
                }
            }

            var response = await httpClient.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                return new Response<TResult>
                {
                    StatusCode = StatusCode.RateLimitExceeded,
                    ErrorMessage = "API returned 429 Too Many Requests"
                };
            }

            var responseToken = JToken.Parse(await response.Content.ReadAsStringAsync());
            var data = responseToken.ToObject<TResult>();
            var isValidResponse = data != null;
            return new Response<TResult>
            {
                StatusCode = isValidResponse ? StatusCode.Ok : StatusCode.OtherError,
                ErrorMessage = isValidResponse ? "" : "Invalid data received.",
                Result = data
            };
        }
        catch (Exception ex)
        {
            return new Response<TResult>
            {
                StatusCode = ex is HttpRequestException httpEx && httpEx.StatusCode != HttpStatusCode.NotFound
                    ? StatusCode.ConnectionError
                    : StatusCode.OtherError,
                ErrorMessage = ex.Message
            };
        }
    }
}