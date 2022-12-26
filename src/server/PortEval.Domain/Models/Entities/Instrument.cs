using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using System;
using PortEval.Domain.Events;

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
        public InstrumentTrackingStatus TrackingStatus { get; private set; }
        public TrackingInformation TrackingInfo { get; private set; }

        internal Instrument(int id, string name, string symbol, string exchange, InstrumentType type, string currencyCode, string note) : this(name, symbol, exchange, type, currencyCode, note)
        {
            Id = id;
        }

        internal Instrument(string name, string symbol, string exchange, InstrumentType type, string currencyCode, string note)
        {
            Name = name;
            Symbol = symbol;
            Exchange = exchange;
            Type = type;
            CurrencyCode = currencyCode;
            Note = note;
            TrackingStatus = InstrumentTrackingStatus.Created;
        }

        public static Instrument Create(string name, string symbol, string exchange, InstrumentType type,
            string currencyCode, string note)
        {
            var instrument = new Instrument(name, symbol, exchange, type, currencyCode, note);
            instrument.AddDomainEvent(new InstrumentCreatedDomainEvent(instrument));

            return instrument;
        }

        public void Rename(string name)
        {
            Name = name;
        }

        public void SetExchange(string exchange)
        {
            Exchange = exchange;
        }

        public void SetNote(string note)
        {
            Note = note;
        }

        public void SetTrackingFrom(DateTime startTime)
        {
            TrackingStatus = InstrumentTrackingStatus.Tracked;
            TrackingInfo = new TrackingInformation(startTime);
        }

        public void SetTrackingStatus(InstrumentTrackingStatus trackingStatus)
        {
            TrackingStatus = trackingStatus;
        }
    }
}
