using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services.BulkImportExport
{
    public interface IImportProcessor<TRow>
    {
        public Task<ImportResult<TRow>> ProcessImport(IEnumerable<TRow> rows);
    }
}
