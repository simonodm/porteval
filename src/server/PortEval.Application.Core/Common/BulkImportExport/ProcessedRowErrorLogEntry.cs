using System.Collections.Generic;
using System.Linq;
using PortEval.Application.Core.Interfaces;

namespace PortEval.Application.Core.Common.BulkImportExport
{
    public class ProcessedRowErrorLogEntry<T> : IErrorLogEntry
    {
        public T Data { get; set; }
        public bool IsError { get; set; }
        public List<string> ErrorMessages { get; set; }

        public ProcessedRowErrorLogEntry(T data)
        {
            Data = data;
            IsError = false;
            ErrorMessages = new List<string>();
        }

        public ProcessedRowErrorLogEntry(T data, string errorMessage) : this(data)
        {
            ErrorMessages.Add(errorMessage);
            IsError = true;
        }

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

        public void AddError(string errorMessage)
        {
            ErrorMessages.Add(errorMessage);
            IsError = true;
        }
    }
}
