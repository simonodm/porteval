using System.Collections.Generic;
using PortEval.Application.Core.Interfaces;

namespace PortEval.Application.Core.Common.BulkImport;

/// <summary>
///     Represents an error log entry for a CSV row which failed to be deserialized.
/// </summary>
public class RawRowErrorLogEntry : IErrorLogEntry
{
    /// <summary>
    ///     An array of original row fields.
    /// </summary>
    public string[] RawRowFields { get; }

    /// <summary>
    ///     Initializes the error log entry.
    /// </summary>
    /// <param name="rawRowFields">An array of original row fields.</param>
    /// <param name="errorMessage">The error message describing why the row failed to be deserialized.</param>
    public RawRowErrorLogEntry(string[] rawRowFields, string errorMessage)
    {
        RawRowFields = rawRowFields;
        ErrorMessages = new List<string> { errorMessage };
    }

    /// <inheritdoc />
    public bool IsError { get; set; } = true;

    /// <inheritdoc />
    public List<string> ErrorMessages { get; set; }
}