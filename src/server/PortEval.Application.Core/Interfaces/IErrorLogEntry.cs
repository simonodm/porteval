using System.Collections.Generic;

namespace PortEval.Application.Core.Interfaces;

/// <summary>
///     Represents a CSV import log entry.
/// </summary>
public interface IErrorLogEntry
{
    /// <summary>
    ///     Determines whether the entry failed to be processed.
    /// </summary>
    public bool IsError { get; set; }

    /// <summary>
    ///     A list of error messages if the entry failed to be processed.
    /// </summary>
    public List<string> ErrorMessages { get; set; }
}