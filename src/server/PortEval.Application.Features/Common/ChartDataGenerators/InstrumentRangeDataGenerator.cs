using PortEval.Application.Features.Extensions;
using PortEval.Application.Features.Interfaces.ChartDataGenerators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common.ChartDataGenerators
{
    /// <inheritdoc cref="IInstrumentRangeDataGenerator"/>
    public class InstrumentRangeDataGenerator : IInstrumentRangeDataGenerator
    {
        private readonly IEnumerator<InstrumentPriceDto> _priceEnumerator;
        private readonly IEnumerator<DateRangeParams> _rangeEnumerator;

        private InstrumentPriceDto _previousPrice;
        private InstrumentPriceDto _currentPrice;
        private bool _finished;

        public InstrumentRangeDataGenerator(IEnumerable<InstrumentPriceDto> prices, IEnumerable<DateRangeParams> ranges)
        {
            _priceEnumerator = prices.GetEnumerator();
            _priceEnumerator.MoveNext();

            _rangeEnumerator = ranges.GetEnumerator();
            _rangeEnumerator.MoveNext();

            _finished = false;
        }

        /// <inheritdoc />
        public InstrumentPriceRangeData GetNextRangeData()
        {
            var currentRange = _rangeEnumerator.Current;

            if (currentRange == null)
            {
                _finished = true;
                return null;
            }
            _rangeEnumerator.MoveNext();

            _currentPrice ??= _priceEnumerator.FindNextElementInEnumerator(p => p.Time <= currentRange.From);

            _previousPrice = _currentPrice;
            var nextPrice = _priceEnumerator.FindNextElementInEnumerator(p => p.Time <= currentRange.To);
            if (nextPrice != null)
            {
                _currentPrice = nextPrice;
            }

            if (_currentPrice == null)
            {
                return null;
            }

            return new InstrumentPriceRangeData
            {
                PriceAtRangeStart = _previousPrice?.Price ?? 0m,
                PriceAtRangeEnd = _currentPrice.Price,
                DateRange = currentRange,
            };
        }

        /// <inheritdoc />
        public bool IsFinished()
        {
            return _finished;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _priceEnumerator.Dispose();
            _rangeEnumerator.Dispose();
        }
    }
}
