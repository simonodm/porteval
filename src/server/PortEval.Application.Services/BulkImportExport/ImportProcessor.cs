using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PortEval.Application.Services.Interfaces.Repositories;

namespace PortEval.Application.Services.BulkImportExport
{
    public abstract class ImportProcessor<TRow, TValidator> : IImportProcessor<TRow>
        where TValidator : AbstractValidator<TRow>, new()
    {
        protected readonly IUnitOfWork UnitOfWork;
        protected readonly TValidator Validator;
        protected Action OnImportFinish;
        protected Func<Task> OnImportFinishAsync;

        protected ImportProcessor(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
            Validator = new TValidator();
        }

        public async Task<ImportResult<TRow>> ProcessImport(IEnumerable<TRow> rows)
        {
            var errorLog = new List<ErrorLogEntry<TRow>>();
            foreach (var row in rows)
            {
                var validationResult = Validator.Validate(row);
                if (!validationResult.IsValid)
                {
                    errorLog.Add(new ErrorLogEntry<TRow>(row, validationResult.Errors.Select(error => error.ErrorMessage)));
                }
                else
                {
                    var processResult = await ProcessItem(row);
                    errorLog.Add(processResult);
                }
            }

            await UnitOfWork.CommitAsync();

            OnImportFinish?.Invoke();
            if (OnImportFinishAsync != null)
            {
                await OnImportFinishAsync();
            }

            return new ImportResult<TRow>
            {
                ErrorLog = errorLog
            };
        }

        public abstract Task<ErrorLogEntry<TRow>> ProcessItem(TRow row);
    }
}
