using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces.Services;

namespace PortEval.Application.Core.Services
{
    public class CsvExportService : ICsvExportService
    {
        public byte[] ConvertToCsv<T>(IEnumerable<T> rows)
        {
            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms);
            using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
            csv.RegisterExportClassMaps();

            csv.WriteRecords(rows);
            sw.Flush();
            return ms.ToArray();
        }
    }
}
