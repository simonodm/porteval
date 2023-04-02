using CsvHelper.Configuration;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Common.BulkImportExport.ClassMaps
{
    public sealed class TransactionClassMap : ClassMap<TransactionDto>
    {
        public TransactionClassMap()
        {
            Map(t => t.Id).Name("Transaction ID").Default(default(int));
            Map(t => t.PositionId).Name("Position ID");
            Map(t => t.Price).Name("Price");
            Map(t => t.Amount).Name("Amount");
            Map(t => t.Time).Name("Time");
        }
    }
}
