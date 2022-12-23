using System.Collections.Generic;

namespace PortEval.Application.Features.Interfaces.Services
{
    public interface ICsvExportService
    {
        public byte[] ConvertToCsv<T>(IEnumerable<T> rows);
    }
}
