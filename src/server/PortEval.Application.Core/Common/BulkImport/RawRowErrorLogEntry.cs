using PortEval.Application.Core.Interfaces;
using System.Collections.Generic;

namespace PortEval.Application.Core.Common.BulkImportExport
{
    public class RawRowErrorLogEntry : IErrorLogEntry
    {
        public string[] RawRowFields { get; set; }
        public bool IsError { get; set; }
        public List<string> ErrorMessages { get; set; }

        public RawRowErrorLogEntry(string[] rawRowFields)
        {
            RawRowFields = rawRowFields;
            IsError = false;
            ErrorMessages = new List<string>();
        }

        public RawRowErrorLogEntry(string[] rawRowFields, string errorMessage) : this(rawRowFields)
        {
            IsError = true;
            ErrorMessages.Add(errorMessage);
        }

        public RawRowErrorLogEntry(string[] rawRowFields, IEnumerable<string> errorMessages) : this(rawRowFields)
        {
            IsError = true;
            ErrorMessages.AddRange(errorMessages);
        }
    }
}
