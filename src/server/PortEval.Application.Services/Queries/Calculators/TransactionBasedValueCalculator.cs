using PortEval.Application.Services.Queries.Calculators.Interfaces;
using PortEval.Application.Services.Queries.Models;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Services.Queries.Calculators
{
    internal class TransactionBasedValueCalculator : ITransactionBasedValueCalculator
    {
        public decimal CalculateValue(IEnumerable<TransactionDetailsQueryModel> transactions, DateTime time)
        {
            var value = 0m;
            foreach (var transaction in transactions)
            {
                if (transaction.Time <= time)
                {
                    value += transaction.Amount * transaction.InstrumentPriceAtRangeEnd;
                }
            }

            return value;
        }
    }
}
