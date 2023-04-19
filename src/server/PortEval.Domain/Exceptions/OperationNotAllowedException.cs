namespace PortEval.Domain.Exceptions;

/// <summary>
///     A domain exception thrown when the requested operation is not permitted by the domain.
/// </summary>
public class OperationNotAllowedException : PortEvalException
{
    public OperationNotAllowedException(string message) : base(message)
    {
    }
}