using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using System;

namespace PortEval.Domain.Models.Entities
{
    public class Instrument : VersionedEntity, IAggregateRoot
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Symbol { get; private set; }
        public string Exchange { get; private set; }
        public InstrumentType Type { get; private set; }
        public string CurrencyCode { get; private set; }
        public string Note { get; private set; }
        public bool IsTracked { get; private set; }
        public TrackingInformation TrackingInfo { get; private set; }

        public Instrument(string name, string symbol, string exchange, InstrumentType type, string currencyCode, string note)
        {
            Name = name;
            Symbol = symbol;
            Exchange = exchange;
            Type = type;
            CurrencyCode = currencyCode;
            Note = note;
            IsTracked = false;
        }

        public void Rename(string name)
        {
            Name = name;
        }

        public void SetNote(string note)
        {
            Note = note;
        }

        public void SetTrackingFrom(DateTime startTime)
        {
            IsTracked = true;
            TrackingInfo = new TrackingInformation(startTime);
        }
    }
}
