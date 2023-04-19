using CsvHelper.Configuration;

namespace PortEval.Application.Models.DTOs.Converters.ClassMaps;

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