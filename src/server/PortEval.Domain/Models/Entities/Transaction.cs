using PortEval.Domain.Exceptions;
using System;

namespace PortEval.Domain.Models.Entities
{
    public class Transaction : VersionedEntity
    {
        public int Id { get; private set; }
        public DateTime Time { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Price { get; private set; }
        public string Note { get; private set; }
        public int PositionId { get; private set; }
        public Position Position { get; private set; }

        public Transaction(int id, int positionId, DateTime time, decimal amount, decimal price, string note = "") : this(positionId, time, amount, price, note)
        {
            Id = id;
        }

        public Transaction(int positionId, DateTime time, decimal amount, decimal price, string note = "")
        {
            if (time < PortEvalConstants.FinancialDataStartTime)
                throw new OperationNotAllowedException(
                    $"Transaction time must be later than {PortEvalConstants.FinancialDataStartTime}");

            if (amount == 0)
                throw new OperationNotAllowedException("Transaction amount cannot be zero.");

            if (price <= 0)
                throw new OperationNotAllowedException("Transaction price must be above zero.");

            PositionId = positionId;
            Time = time;
            Amount = amount;
            Price = price;
            Note = note;
        }

        public void SetTime(DateTime time)
        {
            Time = time;
        }

        public void SetAmount(decimal amount)
        {
            Amount = amount;
        }

        public void SetPrice(decimal price)
        {
            Price = price;
        }

        public void SetNote(string note)
        {
            Note = note;
        }
    }
}
