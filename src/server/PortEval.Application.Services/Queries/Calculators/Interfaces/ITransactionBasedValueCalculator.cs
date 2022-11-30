using PortEval.Application.Services.Queries.Models;
using System;
using System.Collections.Generic;

namespace PortEval.Application.Services.Queries.Calculators.Interfaces
{
    public interface ITransactionBasedValueCalculator
    {
        public decimal CalculateValue(IEnumerable<TransactionDetailsQueryModel> transactions, DateTime time);
    }
}
