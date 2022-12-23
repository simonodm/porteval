using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services.BulkImportExport.Interfaces
{
    public interface IImportProcessor<TRecord>
    {
        public Task<ImportResult<TRecord>> ImportRecords(IEnumerable<TRecord> rows);
    }
}
