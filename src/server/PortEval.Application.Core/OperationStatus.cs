namespace PortEval.Application.Core;

/// <summary>
///     Represents the status of an operation performed by an application service.
/// </summary>
public enum OperationStatus
{
    /// <summary>
    ///     Operation was successful.
    /// </summary>
    Ok,

    /// <summary>
    ///     Operation failed due to invalid input or an application error.
    /// </summary>
    Error,

    /// <summary>
    ///     Operation failed due to the primary entity needed by the operation not existing in the application.
    /// </summary>
    NotFound
}