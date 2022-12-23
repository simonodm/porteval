using PortEval.Application.Features.Extensions;
using PortEval.Application.Features.Interfaces.ChartDataGenerators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    public class PositionChartPointGenerator : IPositionChartPointGenerator
    {
        private readonly IEnumerator<InstrumentPriceDto> _priceEnumerator;
        private readonly IEnumerator<TransactionDto> _transactionEnumerator;
        private readonly IEnumerator<DateRangeParams> _rangeEnumerator;

        private InstrumentPriceDto _firstPrice;
        private InstrumentPriceDto _previousPrice;
        private InstrumentPriceDto _currentPrice;
        private readonly List<TransactionDto> _currentTransactions;

        private bool _finished;

        public PositionChartPointGenerator(PositionPriceListData positionPriceListData,
            IEnumerable<DateRangeParams> ranges)
        {
            _priceEnumerator = positionPriceListData.Prices.GetEnumerator();
            _priceEnumerator.MoveNext();

            _transactionEnumerator = positionPriceListData.Transactions.GetEnumerator();
            _transactionEnumerator.MoveNext();

            _rangeEnumerator = ranges.GetEnumerator();
            _rangeEnumerator.MoveNext();

            _currentTransactions = new List<TransactionDto>();
            _finished = false;
        }

        public PositionChartPointData GetNextChartPointData()
        {
            var currentRange = _rangeEnumerator.Current;
            if (currentRange == null)
            {
                _finished = true;
                return null;
            }
            _rangeEnumerator.MoveNext();

            if (_previousPrice == null) // this is the first run of this method
            {
                var priceAtRangeStart = _priceEnumerator.FindNextElementInEnumerator(p => p.Time <= currentRange.From);
                if (priceAtRangeStart != null)
                {
                    _firstPrice ??= priceAtRangeStart;
                    _previousPrice = priceAtRangeStart;
                    _currentPrice = priceAtRangeStart;
                }
            }

            var nextPrice = _priceEnumerator.FindNextElementInEnumerator(p => p.Time <= currentRange.To);
            if (nextPrice != null)
            {
                _previousPrice = _currentPrice;
                _currentPrice = nextPrice;
            }

            _transactionEnumerator.FindNextElementInEnumerator(
                t => t.Time <= currentRange.To,
                t => _currentTransactions.Add(t)
            );

            if (_currentPrice == null || _currentTransactions.Count == 0)
            {
                return null;
            }

            return new PositionChartPointData
            {
                StartPrice = _firstPrice,
                InstrumentPriceAtRangeStart = _previousPrice,
                InstrumentPriceAtRangeEnd = _currentPrice,
                DateRange = currentRange,
                TransactionsToProcess = _currentTransactions
            };
        }

        public bool IsFinished()
        {
            return _finished;
        }

        public void Dispose()
        {
            _priceEnumerator.Dispose();
            _transactionEnumerator.Dispose();
            _rangeEnumerator.Dispose();
        }
    }
}
