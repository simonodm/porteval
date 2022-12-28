using System;

namespace PortEval.Domain.Models.Entities
{
    public class InstrumentPrice : VersionedEntity, IAggregateRoot
    {
        public int Id { get; private set; }
        public DateTime Time { get; private set; }
        public decimal Price { get; private set; }
        public int InstrumentId { get; private set; }

        internal InstrumentPrice(int id, DateTime time, decimal price, int instrumentId) : this(time, price, instrumentId)
        {
            Id = id;
        }

        internal InstrumentPrice(DateTime time, decimal price, int instrumentId)
        {
            if (time < PortEvalConstants.FinancialDataStartTime)
                throw new InvalidOperationException(
                    $"Price time must be later than {PortEvalConstants.FinancialDataStartTime}");

            if (price <= 0)
                throw new InvalidOperationException("Instrument price must be above zero.");

            Time = time;
            Price = price;
            InstrumentId = instrumentId;
        }

        public static InstrumentPrice Create(DateTime time, decimal price, Instrument instrument)
        {
            return new InstrumentPrice(time, price, instrument.Id);
        }

        public void ChangePrice(decimal newPrice)
        {
            Price = newPrice;
        }
    }
}
