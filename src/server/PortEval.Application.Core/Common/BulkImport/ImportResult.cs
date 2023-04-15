using System.Collections.Generic;

namespace PortEval.Application.Core.Common.BulkImportExport
{
    public class ImportResult<T>
    {
        public IEnumerable<ProcessedRowErrorLogEntry<T>> ErrorLog { get; set; }
    }
}
