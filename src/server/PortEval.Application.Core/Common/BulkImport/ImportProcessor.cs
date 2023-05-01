using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PortEval.Application.Core.Interfaces;

namespace PortEval.Application.Core.Common.BulkImport;

/// <summary>
///     Implements validation and import  of a collection of records.
/// </summary>
/// <typeparam name="TRecord">Type of records to import.</typeparam>
/// <typeparam name="TValidator">Type of the validator.</typeparam>
public abstract class ImportProcessor<TRecord, TValidator> : IImportProcessor<TRecord>
    where TValidator : AbstractValidator<TRecord>, new()
{
    /// <summary>
    ///     A FluentValidation validator to check the correctness of the records.
    /// </summary>
    protected readonly TValidator Validator;

    /// <summary>
    ///     A callback to execute when the import finishes.
    /// </summary>
    protected Action OnImportFinish;

    /// <summary>
    ///     An asynchronous callback to execute when the import finishes.
    /// </summary>
    protected Func<Task> OnImportFinishAsync;

    /// <summary>
    ///     Initializes the import processor.
    /// </summary>
    protected ImportProcessor()
    {
        Validator = new TValidator();
    }

    /// <inheritdoc />
    public async Task<ImportResult<TRecord>> ImportRecordsAsync(IEnumerable<TRecord> records)
    {
        var errorLog = new List<ProcessedRowErrorLogEntry<TRecord>>();
        foreach (var record in records)
        {
            var validationResult = Validator.Validate(record);
            if (!validationResult.IsValid)
            {
                errorLog.Add(new ProcessedRowErrorLogEntry<TRecord>(record,
                    validationResult.Errors.Select(error => error.ErrorMessage)));
            }
            else
            {
                try
                {
                    var processResult = await ProcessItem(record);
                    errorLog.Add(processResult);
                }
                catch (Exception ex)
                {
                    errorLog.Add(new ProcessedRowErrorLogEntry<TRecord>(record, ex.Message));
                }
            }
        }

        OnImportFinish?.Invoke();
        if (OnImportFinishAsync != null)
        {
            await OnImportFinishAsync();
        }

        return new ImportResult<TRecord>
        {
            ErrorLog = errorLog
        };
    }

    /// <summary>
    ///     Imports a single record.
    /// </summary>
    /// <param name="row">Record to import.</param>
    /// <returns>
    ///     A task representing the asynchronous import operation.
    ///     Task result contains an error log entry for the imported record.
    /// </returns>
    protected abstract Task<ProcessedRowErrorLogEntry<TRecord>> ProcessItem(TRecord row);
}