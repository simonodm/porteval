namespace PortEval.Application.Core
{
    public class OperationResponse
    {
        public OperationStatus Status { get; init; } = OperationStatus.Ok;
        public string Message { get; init; }
    }

    public class OperationResponse<T> : OperationResponse
    {
        public T Response { get; init; }
    }
}
