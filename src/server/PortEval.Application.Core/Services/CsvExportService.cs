using CsvHelper;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace PortEval.Application.Core.Services
{
    /// <inheritdoc cref="ICsvExportService" />
    public class CsvExportService : ICsvExportService
    {
        /// <inheritdoc />
        public OperationResponse<byte[]> ConvertToCsv<T>(IEnumerable<T> rows)
        {
            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms);
            using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
            csv.RegisterExportClassMaps();

            csv.WriteRecords(rows);
            sw.Flush();
            return new OperationResponse<byte[]>
            {
                Response = ms.ToArray()
            };
        }
    }
}
