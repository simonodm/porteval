using System.Collections.Generic;

namespace PortEval.Application.Services.BulkImportExport.Interfaces
{
    public interface IErrorLogEntry
    {
        public bool IsError { get; set; }
        public List<string> ErrorMessages { get; set; }
    }
}
