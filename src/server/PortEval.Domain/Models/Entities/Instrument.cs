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
        public TrackingInformation TrackingInfo { get; private set; }

        public IReadOnlyCollection<InstrumentPrice> Prices => _prices.AsReadOnly();
        private readonly List<InstrumentPrice> _prices = new List<InstrumentPrice>();

        public Instrument(string name, string symbol, string exchange, InstrumentType type, string currencyCode)
        {
            Name = name;
            Symbol = symbol;
            Exchange = exchange;
            Type = type;
            CurrencyCode = currencyCode;
        }

        public void SetTrackingFrom(DateTime startTime)
        {
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
            if (price == null)
            {
                throw new ItemNotFoundException($"No price point found for instrument {Id} at time {time}");
            }

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
