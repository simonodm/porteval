using PortEval.Application.Models.DTOs;
using System.Collections.Generic;

namespace PortEval.Application.Features.Interfaces.ChartDataGenerators
{
    public interface ICurrencyConverter
    {
        public IEnumerable<CurrencyExchangeRateDto> CombineExchangeRates(IEnumerable<CurrencyExchangeRateDto> first, IEnumerable<CurrencyExchangeRateDto> second);
        public IEnumerable<EntityChartPointDto> ConvertChartPoints(IEnumerable<EntityChartPointDto> chartPoints, IEnumerable<CurrencyExchangeRateDto> exchangeRates);
        public IEnumerable<TransactionDto> ConvertTransactions(IEnumerable<TransactionDto> transactions, IEnumerable<CurrencyExchangeRateDto> exchangeRates);
        public IEnumerable<InstrumentPriceDto> ConvertInstrumentPrices(IEnumerable<InstrumentPriceDto> instrumentPrices, IEnumerable<CurrencyExchangeRateDto> exchangeRates);
    }
}
