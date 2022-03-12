using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.Enums;
using PortEval.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IReadOnlyCollection<InstrumentPrice> Prices => _prices.AsReadOnly();
        private readonly List<InstrumentPrice> _prices = new List<InstrumentPrice>();

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

        public void SetName(string name)
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

        public InstrumentPrice AddPricePoint(DateTime time, decimal price)
        {
            var newPricePoint = new InstrumentPrice(time, price, Id);

            if (_prices.FirstOrDefault(p => p.Time == newPricePoint.Time) != null)
            {
                throw new OperationNotAllowedException($"Price point already exists for instrument {Id} at time {newPricePoint.Time}");
            }

            _prices.Add(newPricePoint);
            return newPricePoint;
        }

        public InstrumentPrice GetPriceAt(DateTime time)
        {
            var orderedPrices = _prices.OrderBy(p => p.Time).Reverse();
            var price = orderedPrices.FirstOrDefault(p => p.Time <= time);
            return price;
        }

        public void RemovePricePointById(int priceId)
        {
            var price = _prices.FirstOrDefault(p => p.Id == priceId);
            if (price == null)
            {
                throw new ItemNotFoundException($"No price point found with id {priceId} for instrument {Id}.");
            }

            _prices.Remove(price);
        }
    }
}
