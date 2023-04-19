namespace PortEval.Application.Core;

/// <summary>
///     Represents the response of an operation performed by an application service.
/// </summary>
public class OperationResponse
{
    /// <summary>
    ///     Determines the status of the operation.
    /// </summary>
    public OperationStatus Status { get; init; } = OperationStatus.Ok;

    /// <summary>
    ///     Contains an error message if there is one.
    /// </summary>
    public string Message { get; init; }
}

/// <summary>
///     Represents the response of an operation performed by an application service.
/// </summary>
/// <typeparam name="T">Type of the operation result.</typeparam>
public class OperationResponse<T> : OperationResponse
{
    /// <summary>
    ///     Result of the operation.
    /// </summary>
    public T Response { get; init; }
}