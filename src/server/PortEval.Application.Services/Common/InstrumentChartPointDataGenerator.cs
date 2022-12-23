using PortEval.Application.Features.Extensions;
using PortEval.Application.Features.Interfaces.ChartDataGenerators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    /// <inheritdoc />
    public class InstrumentChartPointDataGenerator : IInstrumentChartPointGenerator
    {
        private readonly IEnumerator<InstrumentPriceDto> _priceEnumerator;
        private readonly IEnumerator<DateRangeParams> _rangeEnumerator;

        private InstrumentPriceDto _firstPrice;
        private InstrumentPriceDto _currentPrice;
        private bool _finished;

        public InstrumentChartPointDataGenerator(IEnumerable<InstrumentPriceDto> prices, IEnumerable<DateRangeParams> ranges)
        {
            _priceEnumerator = prices.GetEnumerator();
            _priceEnumerator.MoveNext();

            _rangeEnumerator = ranges.GetEnumerator();
            _rangeEnumerator.MoveNext();

            _finished = false;
        }

        /// <inheritdoc />
        public InstrumentPriceChartPointData GetNextChartPointData()
        {
            var currentRange = _rangeEnumerator.Current;

            if (currentRange == null)
            {
                _finished = true;
                return null;
            }
            _rangeEnumerator.MoveNext();
            
            var nextPrice = _priceEnumerator.FindNextElementInEnumerator(p => p.Time <= currentRange.To);
            if (nextPrice != null)
            {
                _firstPrice ??= nextPrice;
                _currentPrice = nextPrice;
            }

            if (_currentPrice == null)
            {
                return null;
            }

            return new InstrumentPriceChartPointData
            {
                StartPrice = _firstPrice.Price,
                Price = _currentPrice.Price,
                Time = currentRange.To,
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
