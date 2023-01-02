using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Events
{
    public class TransactionChangedDomainEvent : IDomainEvent
    {
        public Transaction Transaction { get; init; }
        public Position Position { get; init; }

        public TransactionChangedDomainEvent(Transaction transaction, Position position)
        {
            Transaction = transaction;
            Position = position;
        }
    }
}
