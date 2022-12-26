using PortEval.Application.Features.Services.BulkImportExport;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces
{
    public interface IImportProcessor<TRecord>
    {
        public Task<ImportResult<TRecord>> ImportRecords(IEnumerable<TRecord> rows);
    }
}
