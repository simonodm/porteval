using FluentValidation;
using PortEval.Application.Features.Services.BulkImportExport.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services.BulkImportExport
{
    public abstract class ImportProcessor<TRecord, TValidator> : IImportProcessor<TRecord>
        where TValidator : AbstractValidator<TRecord>, new()
    {
        protected readonly TValidator Validator;
        protected Action OnImportFinish;
        protected Func<Task> OnImportFinishAsync;

        protected ImportProcessor()
        {
            Validator = new TValidator();
        }

        public async Task<ImportResult<TRecord>> ImportRecords(IEnumerable<TRecord> records)
        {
            var errorLog = new List<ProcessedRowErrorLogEntry<TRecord>>();
            foreach (var record in records)
            {
                var validationResult = Validator.Validate(record);
                if (!validationResult.IsValid)
                {
                    errorLog.Add(new ProcessedRowErrorLogEntry<TRecord>(record, validationResult.Errors.Select(error => error.ErrorMessage)));
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

        protected abstract Task<ProcessedRowErrorLogEntry<TRecord>> ProcessItem(TRecord row);
    }
}
