using PortEval.Application.Services.Queries.Calculators.Interfaces;
using PortEval.Application.Services.Queries.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortEval.Application.Services.Queries.Calculators
{
    internal class TransactionBasedProfitCalculator : ITransactionBasedProfitCalculator
    {
        public decimal CalculateProfit(IEnumerable<TransactionDetailsQueryModel> transactions, DateTime from,
            DateTime to)
        {
            var realizedProfitAtRangeEnd = 0m;
            var unrealizedProfitAtRangeStart = 0m;
            var unrealizedProfitAtRangeEnd = 0m;

            foreach (var transaction in transactions)
            {
                unrealizedProfitAtRangeEnd += transaction.Amount * transaction.InstrumentPriceAtRangeEnd;

                if (transaction.Time < from)
                {
                    unrealizedProfitAtRangeStart += transaction.Amount * transaction.InstrumentPriceAtRangeStart;
                }
                else if (transaction.Time <= to)
                {
                    realizedProfitAtRangeEnd -= transaction.Amount * transaction.Price;
                }
            }

            return realizedProfitAtRangeEnd + unrealizedProfitAtRangeEnd -
                   unrealizedProfitAtRangeStart;
        }
    }
}
