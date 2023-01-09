using PortEval.DataFetcher.Interfaces;
using System;

namespace PortEval.DataFetcher
{
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
