using System.Collections.Generic;
using System.Linq;
using PortEval.Application.Core.Interfaces;

namespace PortEval.Application.Core.Common.BulkImport;

/// <summary>
///     Represents an error log entry of a CSV row which was successfully parsed.
/// </summary>
/// <typeparam name="T">Type of record represented by the row.</typeparam>
public class ProcessedRowErrorLogEntry<T> : IErrorLogEntry
{
    /// <summary>
    ///     A reference to the original record.
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    ///     Initializes the error log entry for successfully processed record.
    /// </summary>
    /// <param name="data">A reference to the original record.</param>
    public ProcessedRowErrorLogEntry(T data)
    {
        Data = data;
        IsError = false;
        ErrorMessages = new List<string>();
    }

    /// <summary>
    ///     Initializes the error log entry with an error message.
    /// </summary>
    /// <param name="data">A reference to the original record.</param>
    /// <param name="errorMessage">Error message describing why the record was not imported.</param>
    public ProcessedRowErrorLogEntry(T data, string errorMessage) : this(data)
    {
        ErrorMessages.Add(errorMessage);
        IsError = true;
    }

    /// <summary>
    ///     Initializes the error log entry with a collection of error messages.
    /// </summary>
    /// <param name="data">A reference to the original record.</param>
    /// <param name="errorMessages">A collection of error messages describing why the error was not imported.</param>
    public ProcessedRowErrorLogEntry(T data, IEnumerable<string> errorMessages) : this(data)
    {
        if (errorMessages.Any())
        {
            ErrorMessages.AddRange(errorMessages);
            IsError = true;
        }
        else
        {
            IsError = false;
        }
    }

    /// <inheritdoc />
    public bool IsError { get; set; }

    /// <inheritdoc />
    public List<string> ErrorMessages { get; set; }

    /// <summary>
    ///     Adds an error to the log entry.
    /// </summary>
    /// <param name="errorMessage">Error message to add.</param>
    public void AddError(string errorMessage)
    {
        ErrorMessages.Add(errorMessage);
        IsError = true;
    }
}