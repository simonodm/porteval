﻿using PortEval.Domain.Exceptions;
using PortEval.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortEval.Domain.Models.Entities
{
    public class Currency : VersionedEntity, IAggregateRoot
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public string Symbol { get; private set; }
        public bool IsDefault { get; private set; }
        public TrackingInformation TrackingInfo { get; private set; }

        public IReadOnlyCollection<CurrencyExchangeRate> ExchangeRates => _exchangeRates.AsReadOnly();
        private readonly List<CurrencyExchangeRate> _exchangeRates = new List<CurrencyExchangeRate>();

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

        public CurrencyExchangeRate AddExchangeRate(DateTime time, string currencyToCode, decimal rate)
        {
            var exchangeRate = new CurrencyExchangeRate(time, rate, Code, currencyToCode);

            if (_exchangeRates.FirstOrDefault(r => r.Time == exchangeRate.Time && r.CurrencyToCode == exchangeRate.CurrencyToCode) != null)
            {
                throw new OperationNotAllowedException($"Exchange rate from {Code} to {exchangeRate.CurrencyToCode} already exists at {exchangeRate.Time}.");
            }

            _exchangeRates.Add(exchangeRate);
            return exchangeRate;
        }

        public CurrencyExchangeRate GetExchangeRate(string currencyToCode, DateTime time)
        {
            var orderedExchangeRates = _exchangeRates.Where(rate => rate.CurrencyToCode == currencyToCode).OrderByDescending(rate => rate.Time);
            var exchangeRate =
                orderedExchangeRates.FirstOrDefault(rate => rate.Time <= time);
            return exchangeRate;
        }

        public void RemoveExchangeRate(int id)
        {
            var existingRate = _exchangeRates.FirstOrDefault(r => r.Id == id);
            if (existingRate == null)
            {
                throw new ItemNotFoundException($"No exchange rate {id} found in currency {Code}.");
            }

            _exchangeRates.Remove(existingRate);
        }
    }
}
