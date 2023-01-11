using PortEval.Domain.Models.Entities;

namespace PortEval.Domain.Events
{
    public class TransactionAddedToPositionDomainEvent : IDomainEvent
    {
        public Transaction Transaction { get; init; }
        public Position Position { get; init; }

        public TransactionAddedToPositionDomainEvent(Transaction transaction, Position position)
        {
            Transaction = transaction;
            Position = position;
        }
    }
}
