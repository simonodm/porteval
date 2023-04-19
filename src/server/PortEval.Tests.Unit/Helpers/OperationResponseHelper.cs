using PortEval.Application.Core;

namespace PortEval.Tests.Unit.Helpers;

internal static class OperationResponseHelper
{
    public static OperationResponse<T> GenerateSuccessfulOperationResponse<T>(T result)
    {
        return new OperationResponse<T>
        {
            Response = result
        };
    }

    public static OperationResponse GenerateSuccessfulOperationResponse()
    {
        return new OperationResponse();
    }

    public static OperationResponse<T> GenerateNotFoundOperationResponse<T>()
    {
        return new OperationResponse<T>
        {
            Status = OperationStatus.NotFound,
            Response = default
        };
    }

    public static OperationResponse GenerateNotFoundOperationResponse()
    {
        return new OperationResponse
        {
            Status = OperationStatus.NotFound
        };
    }

    public static OperationResponse<T> GenerateErrorOperationResponse<T>()
    {
        return new OperationResponse<T>
        {
            Status = OperationStatus.Error,
            Response = default
        };
    }

    public static OperationResponse GenerateErrorOperationResponse()
    {
        return new OperationResponse
        {
            Status = OperationStatus.Error
        };
    }
}