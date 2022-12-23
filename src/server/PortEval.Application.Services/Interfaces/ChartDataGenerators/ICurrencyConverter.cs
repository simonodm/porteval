using PortEval.Application.Models.DTOs;
using System.Collections.Generic;

namespace PortEval.Application.Features.Interfaces.ChartDataGenerators
{
    /// <summary>
    /// Converts common data collections to a different currency based on the provided exchange rates.
    /// </summary>
    public interface ICurrencyConverter
    {
        /// <summary>
        /// Combines the provided collections of exchange rates by multiplying them.
        /// </summary>
        /// <param name="first">First exchange rates.</param>
        /// <param name="second">Second exchange rates.</param>
        /// <returns>
        /// A collection of exchange rates created by multiplying the provided collections. For example, at time T it will contain the product
        /// of the exchange rate at T from the first collection and the exchange rate at T from the second collection.
        /// </returns>
        public IEnumerable<CurrencyExchangeRateDto> CombineExchangeRates(IEnumerable<CurrencyExchangeRateDto> first, IEnumerable<CurrencyExchangeRateDto> second);
        
        /// <summary>
        /// Converts the provided chart points based on the provided exchange rates.
        /// </summary>
        /// <param name="chartPoints">Chart points to convert.</param>
        /// <param name="exchangeRates">Exchange rates to convert by.</param>
        /// <returns>A collection of chart points with their values multiplied by the appropriate exchange rates from <paramref name="exchangeRates"/>.</returns>
        public IEnumerable<EntityChartPointDto> ConvertChartPoints(IEnumerable<EntityChartPointDto> chartPoints, IEnumerable<CurrencyExchangeRateDto> exchangeRates);

        /// <summary>
        /// Converts the provided transactions based on the provided exchange rates.
        /// </summary>
        /// <param name="transactions">Transactions to convert.</param>
        /// <param name="exchangeRates">Exchange rates to convert by.</param>
        /// <returns>A collection of transactions with their prices multiplied by the appropriate exchange rates from <paramref name="exchangeRates"/>.</returns>
        public IEnumerable<TransactionDto> ConvertTransactions(IEnumerable<TransactionDto> transactions, IEnumerable<CurrencyExchangeRateDto> exchangeRates);

        /// <summary>
        /// Converts the provided prices based on the provided exchange rates.
        /// </summary>
        /// <param name="instrumentPrices">Instrument prices to convert.</param>
        /// <param name="exchangeRates">Exchange rates to convert by.</param>
        /// <returns>A collection of prices multiplied by the appropriate exchange rates from <paramref name="exchangeRates"/>.</returns>
        public IEnumerable<InstrumentPriceDto> ConvertInstrumentPrices(IEnumerable<InstrumentPriceDto> instrumentPrices, IEnumerable<CurrencyExchangeRateDto> exchangeRates);
    }
}
