using PortEval.Domain.Events;
using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortEval.Domain.Models.Entities
{
    public class Position : VersionedEntity, IAggregateRoot
    {
        public int Id { get; private set; }
        public string Note { get; private set; }
        public int PortfolioId { get; private set; }
        public int InstrumentId { get; private set; }

        public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();
        private readonly List<Transaction> _transactions = new List<Transaction>();

        internal Position(int id, int portfolioId, int instrumentId, string note) : this(portfolioId, instrumentId, note)
        {
            Id = id;
        }

        internal Position(int portfolioId, int instrumentId, string note)
        {
            PortfolioId = portfolioId;
            InstrumentId = instrumentId;
            Note = note;
        }

        public static Position Create(Portfolio portfolio, Instrument instrument, string note)
        {
            if (instrument.Type == InstrumentType.Index)
            {
                throw new OperationNotAllowedException("Cannot open a position for an index.");
            }

            return new Position(portfolio.Id, instrument.Id, note);
        }

        public Transaction FindTransaction(int transactionId)
        {
            return _transactions.FirstOrDefault(t => t.Id == transactionId);
        }

        public Transaction AddTransaction(decimal amount, decimal price, DateTime time, string note = "")
        {
            var transaction = Transaction.Create(Id, time, amount, price, note);

            if (GetAmountAt(time) + transaction.Amount < 0)
            {
                throw new OperationNotAllowedException($"Failed to add transaction to position {Id}: position amount cannot fall below zero.");
            }

            _transactions.Add(transaction);
            AddDomainEvent(new TransactionAddedToPositionDomainEvent(transaction, this));
            return transaction;
        }

        public Transaction UpdateTransaction(int id, decimal amount, decimal price, DateTime time, string note)
        {
            var transaction = FindTransaction(id);
            if (transaction == null)
            {
                throw new ItemNotFoundException($"Transaction {id} not found in position {Id}.");
            }

            if ((transaction.Time > time && GetAmountAt(time) + amount < 0)
                || (transaction.Time <= time && GetAmountAt(time) - transaction.Amount + amount < 0))
            {
                throw new OperationNotAllowedException($"Position amount cannot fall below zero.");
            }

            transaction.SetTime(time);
            transaction.SetAmount(amount);
            transaction.SetPrice(price);
            transaction.SetNote(note);

            AddDomainEvent(new TransactionChangedDomainEvent(transaction, this));
            return transaction;
        }

        public void RemoveTransaction(int transactionId)
        {
            var transaction = FindTransaction(transactionId);
            if (transaction == null)
            {
                throw new ItemNotFoundException($"Position {Id} does not contain transaction {transactionId}.");
            }

            // It is necessary to check whether removing the transaction causes position amount to fall below zero at any point after the transaction.
            var amount = GetAmountAt(transaction.Time) - transaction.Amount;
            foreach (var existingTransaction in _transactions.OrderBy(t => t.Time).Where(t => t.Time > transaction.Time))
            {
                amount += existingTransaction.Amount;
                if (amount < 0)
                {
                    throw new OperationNotAllowedException(
                        $"Failed to remove transaction {transactionId}: position amount cannot fall below zero.");
                }
            }

            _transactions.Remove(transaction);
        }

        public void SetNote(string note)
        {
            Note = note;
        }

        private decimal GetAmountAt(DateTime time)
        {
            return _transactions.Where(t => t.Time <= time).Sum(t => t.Amount);
        }
    }
}
