namespace PortEval.Domain.Exceptions;

/// <summary>
///     A domain exception thrown when an entity needed to execute a domain operation is not found.
/// </summary>
public class ItemNotFoundException : PortEvalException
{
    public ItemNotFoundException(string message) : base(message)
    {
    }
}