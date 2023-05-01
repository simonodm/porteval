using System.Collections.Generic;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Common.ChartDataGenerators;

/// <inheritdoc cref="IInstrumentRangeDataGenerator" />
public class InstrumentRangeDataGenerator : IInstrumentRangeDataGenerator
{
    private readonly IEnumerator<InstrumentPriceDto> _priceEnumerator;
    private readonly IEnumerator<DateRangeParams> _rangeEnumerator;
    private InstrumentPriceDto _currentPrice;
    private bool _finished;

    private InstrumentPriceDto _previousPrice;

    /// <summary>
    ///     Initializes the range data generator.
    /// </summary>
    /// <param name="prices">Prices of the instrument.</param>
    /// <param name="ranges">Ranges for which range data should be calculated.</param>
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
            DateRange = currentRange
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