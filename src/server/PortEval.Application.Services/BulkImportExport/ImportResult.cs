using System.Collections.Generic;

namespace PortEval.Application.Services.BulkImportExport
{
    public class ImportResult<T>
    {
        public IEnumerable<ErrorLogEntry<T>> ErrorLog { get; set; }
    }
}
