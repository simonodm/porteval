﻿using System.Collections.Generic;
using PortEval.Application.Core.Extensions;
using PortEval.Application.Core.Interfaces.ChartDataGenerators;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;

namespace PortEval.Application.Core.Common.ChartDataGenerators;

/// <inheritdoc />
public class PositionRangeDataGenerator : IPositionRangeDataGenerator
{
    private readonly List<TransactionDto> _currentTransactions;

    private readonly int _positionId;
    private readonly IEnumerator<InstrumentPriceDto> _priceEnumerator;
    private readonly IEnumerator<DateRangeParams> _rangeEnumerator;
    private readonly IEnumerator<TransactionDto> _transactionEnumerator;
    private InstrumentPriceDto _currentPrice;

    private bool _finished;

    private InstrumentPriceDto _previousPrice;

    /// <summary>
    ///     Initializes the range data generator.
    /// </summary>
    /// <param name="positionPriceListData">A wrapper over the position's transactions and the underlying instrument's prices.</param>
    /// <param name="ranges">Ranges for which range data should be generated.</param>
    public PositionRangeDataGenerator(PositionPriceListData positionPriceListData,
        IEnumerable<DateRangeParams> ranges)
    {
        _priceEnumerator = positionPriceListData.Prices.GetEnumerator();
        _priceEnumerator.MoveNext();

        _transactionEnumerator = positionPriceListData.Transactions.GetEnumerator();
        _transactionEnumerator.MoveNext();

        _rangeEnumerator = ranges.GetEnumerator();
        _rangeEnumerator.MoveNext();

        _positionId = positionPriceListData.PositionId;

        _currentTransactions = new List<TransactionDto>();
        _finished = false;
    }

    /// <inheritdoc />
    public PositionPriceRangeData GetNextRangeData()
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

        _transactionEnumerator.FindNextElementInEnumerator(
            t => t.Time <= currentRange.To,
            t => _currentTransactions.Add(t)
        );

        if (_currentPrice == null || _currentTransactions.Count == 0)
        {
            return null;
        }

        return new PositionPriceRangeData
        {
            PositionId = _positionId,
            PriceAtRangeStart = _previousPrice,
            PriceAtRangeEnd = _currentPrice,
            DateRange = currentRange,
            Transactions = _currentTransactions
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
        _transactionEnumerator.Dispose();
        _rangeEnumerator.Dispose();
    }
}