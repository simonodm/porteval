using System;

namespace PortEval.Domain.Exceptions;

/// <summary>
///     A base class for domain exceptions.
/// </summary>
public abstract class PortEvalException : Exception
{
    protected PortEvalException(string message) : base(message)
    {
    }
}