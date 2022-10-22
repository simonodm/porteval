using CsvHelper;
using PortEval.Application.Services.BulkImportExport.ClassMaps;
using PortEval.Application.Services.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using PortEval.Application.Services.Extensions;

namespace PortEval.Application.Services
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
