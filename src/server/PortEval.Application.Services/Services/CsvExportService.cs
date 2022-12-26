using CsvHelper;
using PortEval.Application.Features.Extensions;
using PortEval.Application.Features.Interfaces.Services;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace PortEval.Application.Features.Services
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
