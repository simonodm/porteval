using System.Collections.Generic;
using System.Linq;

namespace PortEval.Application.Services.BulkImportExport
{
    public class ErrorLogEntry<T>
    {
        public T Row { get; set; }
        public bool IsError { get; set; }
        public List<string> ErrorMessages { get; set; }

        public ErrorLogEntry(T row)
        {
            Row = row;
            IsError = false;
            ErrorMessages = new List<string>();
        }

        public ErrorLogEntry(T row, IEnumerable<string> errorMessages) : this(row)
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
