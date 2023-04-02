using PortEval.DataFetcher.Interfaces;
using System;

namespace PortEval.DataFetcher
{
    /// <summary>
    /// Marks a data source method as being capable of processing a specific type of request returning a specific type of response.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestProcessorAttribute : Attribute
    {
        public Type RequestType { get; private set; }
        public Type ResultType { get; private set; }

        public RequestProcessorAttribute(Type requestType, Type resultType)
        {
            ValidateTypeIsRequest(requestType);

            RequestType = requestType;
            ResultType = resultType;
        }

        /// <summary>
        /// Determines whether a method marked by this attribute can process the specified request/response pair.
        /// </summary>
        /// <typeparam name="TRequest">Type of request to process.</typeparam>
        /// <typeparam name="TResult">Type of response returned by the method.</typeparam>
        /// <returns><c>true</c> if the marked method accepts a request of type <typeparamref name="TRequest"/> and returns a result of type <typeparamref name="TResult"/>.</returns>
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
}
