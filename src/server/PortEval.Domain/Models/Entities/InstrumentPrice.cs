using System;

namespace PortEval.Domain.Models.Entities
{
    public class InstrumentPrice : IAggregateRoot
    {
        public int Id { get; private set; }
        public DateTime Time { get; private set; }
        public decimal Price { get; private set; }
        public int InstrumentId { get; private set; }

        public InstrumentPrice(DateTime time, decimal price, int instrumentId)
        {
            Time = time;
            Price = price;
            InstrumentId = instrumentId;
        }
    }
}
