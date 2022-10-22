using System.Collections.Generic;

namespace PortEval.Application.Services.Interfaces
{
    public interface ICsvExportService
    {
        public byte[] ConvertToCsv<T>(IEnumerable<T> rows);
    }
}
