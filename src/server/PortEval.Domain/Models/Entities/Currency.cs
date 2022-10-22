using PortEval.Domain.Models.ValueObjects;
using System;

namespace PortEval.Domain.Models.Entities
{
    public class Currency : VersionedEntity, IAggregateRoot
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Symbol { get; private set; }
        public bool IsDefault { get; private set; }
        public TrackingInformation TrackingInfo { get; private set; }

        public Currency(string code, string name, string symbol, bool isDefault = false)
        {
            Code = code;
            Name = name;
            Symbol = symbol;
            IsDefault = isDefault;
        }

        public void SetAsDefault()
        {
            IsDefault = true;
        }

        public void UnsetDefault()
        {
            IsDefault = false;
        }

        public void SetTrackingFrom(DateTime time)
        {
            TrackingInfo = new TrackingInformation(time);
        }
    }
}
