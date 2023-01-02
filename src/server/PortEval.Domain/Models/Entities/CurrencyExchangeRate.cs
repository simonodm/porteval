using System;

namespace PortEval.Domain.Models.Entities
{
    public class CurrencyExchangeRate : Entity, IAggregateRoot
    {
        public int Id { get; private set; }
        public DateTime Time { get; private set; }
        public decimal ExchangeRate { get; private set; }
        public string CurrencyFromCode { get; private set; }
        public string CurrencyToCode { get; private set; }

        internal CurrencyExchangeRate(int id, DateTime time, decimal exchangeRate, string currencyFromCode, string currencyToCode) : this(time, exchangeRate, currencyFromCode, currencyToCode)
        {
            Id = id;
        }

        internal CurrencyExchangeRate(DateTime time, decimal exchangeRate, string currencyFromCode, string currencyToCode)
        {
            if (time < PortEvalConstants.FinancialDataStartTime)
                throw new InvalidOperationException(
                    $"Exchange rate time must be later than {PortEvalConstants.FinancialDataStartTime}");

            Time = time;
            ExchangeRate = exchangeRate;
            CurrencyFromCode = currencyFromCode;
            CurrencyToCode = currencyToCode;
        }

        public static CurrencyExchangeRate Create(DateTime time, decimal exchangeRate, Currency currencyFrom, Currency currencyTo)
        {
            return new CurrencyExchangeRate(time, exchangeRate, currencyFrom.Code, currencyTo.Code);
        }
    }
}
