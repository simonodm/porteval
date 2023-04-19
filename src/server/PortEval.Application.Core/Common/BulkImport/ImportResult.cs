using System.Collections.Generic;

namespace PortEval.Application.Core.Common.BulkImport;

/// <summary>
///     Represents the result of a single bulk import.
/// </summary>
/// <typeparam name="T">Type of records which were imported.</typeparam>
public class ImportResult<T>
{
    /// <summary>
    ///     Contains error log entries corresponding to imported records.
    /// </summary>
    public IEnumerable<ProcessedRowErrorLogEntry<T>> ErrorLog { get; init; }
}