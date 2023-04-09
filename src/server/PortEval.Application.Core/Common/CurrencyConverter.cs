using System.Collections.Generic;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Exceptions;

namespace PortEval.Application.Core.Common
{
    /// <inheritdoc cref="ICurrencyConverter" />
    public class CurrencyConverter : ICurrencyConverter
    {
        /// <inheritdoc />
        public IEnumerable<CurrencyExchangeRateDto> CombineExchangeRates(IEnumerable<CurrencyExchangeRateDto> first, IEnumerable<CurrencyExchangeRateDto> second)
        {
            var result = new List<CurrencyExchangeRateDto>();

            using var firstExchangeRateEnumerator = first.GetEnumerator();
            firstExchangeRateEnumerator.MoveNext();

            CurrencyExchangeRateDto previousExchangeRate = null;

            foreach (var exchangeRate in second)
            {
                var currentFirstExchangeRate = firstExchangeRateEnumerator.Current;
                while (currentFirstExchangeRate != null && currentFirstExchangeRate.Time <= exchangeRate.Time)
                {
                    firstExchangeRateEnumerator.MoveNext();
                    previousExchangeRate = currentFirstExchangeRate;
                    currentFirstExchangeRate = firstExchangeRateEnumerator.Current;
                }

                if (previousExchangeRate == null)
                {
                    throw new ItemNotFoundException($"No exchange rate available at {exchangeRate.Time}.");
                }

                result.Add(new CurrencyExchangeRateDto
                {
                    CurrencyFromCode = previousExchangeRate.CurrencyFromCode,
                    CurrencyToCode = exchangeRate.CurrencyToCode,
                    ExchangeRate = previousExchangeRate.ExchangeRate * exchangeRate.ExchangeRate,
                    Time = exchangeRate.Time
                });
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<EntityChartPointDto> ConvertChartPoints(IEnumerable<EntityChartPointDto> chartPoints, IEnumerable<CurrencyExchangeRateDto> exchangeRates)
        {
            var result = new List<EntityChartPointDto>();

            using var exchangeRateEnumerator = exchangeRates.GetEnumerator();
            exchangeRateEnumerator.MoveNext();

            CurrencyExchangeRateDto previousExchangeRate = null;

            foreach (var point in chartPoints)
            {
                var currentExchangeRate = exchangeRateEnumerator.Current;
                while (currentExchangeRate != null && currentExchangeRate.Time <= point.Time)
                {
                    exchangeRateEnumerator.MoveNext();
                    previousExchangeRate = currentExchangeRate;
                    currentExchangeRate = exchangeRateEnumerator.Current;
                }

                if (previousExchangeRate == null)
                {
                    throw new ItemNotFoundException($"No exchange rate available at {point.Time}.");
                }

                result.Add(point.ChangeValue(point.Value * previousExchangeRate.ExchangeRate));
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<TransactionDto> ConvertTransactions(IEnumerable<TransactionDto> transactions, IEnumerable<CurrencyExchangeRateDto> exchangeRates)
        {
            var result = new List<TransactionDto>();

            using var exchangeRateEnumerator = exchangeRates.GetEnumerator();
            exchangeRateEnumerator.MoveNext();

            CurrencyExchangeRateDto previousExchangeRate = null;

            foreach (var transaction in transactions)
            {
                var currentExchangeRate = exchangeRateEnumerator.Current;
                while (currentExchangeRate != null && currentExchangeRate.Time <= transaction.Time)
                {
                    exchangeRateEnumerator.MoveNext();
                    previousExchangeRate = currentExchangeRate;
                    currentExchangeRate = exchangeRateEnumerator.Current;
                }

                if (previousExchangeRate == null)
                {
                    throw new ItemNotFoundException($"No exchange rate available at {transaction.Time}.");
                }

                result.Add(new TransactionDto
                {
                    Id = transaction.Id,
                    Amount = transaction.Amount,
                    Instrument = transaction.Instrument,
                    Note = transaction.Note,
                    PortfolioId = transaction.PortfolioId,
                    PositionId = transaction.PositionId,
                    Time = transaction.Time,
                    Price = transaction.Price * previousExchangeRate.ExchangeRate
                });
            }

            return result;
        }

        public IEnumerable<InstrumentPriceDto> ConvertInstrumentPrices(IEnumerable<InstrumentPriceDto> instrumentPrices, IEnumerable<CurrencyExchangeRateDto> exchangeRates)
        {
            var result = new List<InstrumentPriceDto>();

            using var exchangeRateEnumerator = exchangeRates.GetEnumerator();
            exchangeRateEnumerator.MoveNext();

            CurrencyExchangeRateDto previousExchangeRate = null;

            foreach (var instrumentPrice in instrumentPrices)
            {
                var currentExchangeRate = exchangeRateEnumerator.Current;
                while (currentExchangeRate != null && currentExchangeRate.Time <= instrumentPrice.Time)
                {
                    exchangeRateEnumerator.MoveNext();
                    previousExchangeRate = currentExchangeRate;
                    currentExchangeRate = exchangeRateEnumerator.Current;
                }

                if (previousExchangeRate == null)
                {
                    throw new ItemNotFoundException($"No exchange rate available at {instrumentPrice.Time}.");
                }

                result.Add(new InstrumentPriceDto
                {
                    Id = instrumentPrice.Id,
                    InstrumentId = instrumentPrice.InstrumentId,
                    Time = instrumentPrice.Time,
                    Price = instrumentPrice.Price * previousExchangeRate.ExchangeRate
                });
            }

            return result;
        }
    }
}
