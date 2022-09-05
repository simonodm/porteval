using System.Collections.Generic;

namespace PortEval.Application.Services.BulkImportExport
{
    public class ImportResult<T>
    {
        public IEnumerable<ProcessedRowErrorLogEntry<T>> ErrorLog { get; set; }
    }
}
