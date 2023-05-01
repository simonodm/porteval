using System;
using PortEval.DataFetcher.Interfaces;

namespace PortEval.DataFetcher;

/// <summary>
///     Marks a data source method as being capable of processing a specific type of request returning a specific type of
///     response.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class RequestProcessorAttribute : Attribute
{
    /// <summary>
    ///     Type of supported requests.
    /// </summary>
    public Type RequestType { get; }

    /// <summary>
    ///     Type of expected responses.
    /// </summary>
    public Type ResultType { get; }

    /// <summary>
    ///     Marks a data source method as being capable of processing a specific type of request returning a specific type of
    ///     response.
    /// </summary>
    /// <param name="requestType">Type of supported requests.</param>
    /// <param name="resultType">Type of expected responses.</param>
    public RequestProcessorAttribute(Type requestType, Type resultType)
    {
        ValidateTypeIsRequest(requestType);

        RequestType = requestType;
        ResultType = resultType;
    }

    /// <summary>
    ///     Determines whether a method marked by this attribute can process the specified request/response pair.
    /// </summary>
    /// <typeparam name="TRequest">Type of request to process.</typeparam>
    /// <typeparam name="TResult">Type of response returned by the method.</typeparam>
    /// <returns>
    ///     <c>true</c> if the marked method accepts a request of type <typeparamref name="TRequest" /> and returns a
    ///     result of type <typeparamref name="TResult" />.
    /// </returns>
    public bool CanProcess<TRequest, TResult>()
    {
        return RequestType == typeof(TRequest) &&
               ResultType == typeof(TResult);
    }

    private static void ValidateTypeIsRequest(Type type)
    {
        if (!type.IsAssignableTo(typeof(IRequest)))
        {
            throw new ArgumentException($"{type} does not implement {nameof(IRequest)}.");
        }
    }
}