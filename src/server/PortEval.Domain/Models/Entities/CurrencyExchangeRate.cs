﻿using System;

namespace PortEval.Domain.Models.Entities
{
    public class CurrencyExchangeRate
    {
        public int Id { get; private set; }
        public DateTime Time { get; private set; }
        public decimal ExchangeRate { get; private set; }
        public string CurrencyFromCode { get; private set; }
        public Currency CurrencyFrom { get; private set; }
        public string CurrencyToCode { get; private set; }
        public Currency CurrencyTo { get; private set; }

        public CurrencyExchangeRate(DateTime time, decimal exchangeRate, string currencyFromCode, string currencyToCode)
        {
            Time = time;
            ExchangeRate = exchangeRate;
            CurrencyFromCode = currencyFromCode;
            CurrencyToCode = currencyToCode;
        }
    }
}
