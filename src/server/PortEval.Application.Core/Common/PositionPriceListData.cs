﻿using System.Collections.Generic;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Common;

/// <summary>
///     Represents a collection of a position's transactions and the corresponding instrument's prices.
/// </summary>
public class PositionPriceListData
{
    /// <summary>
    ///     ID of the position.
    /// </summary>
    public int PositionId { get; set; }

    /// <summary>
    ///     Position's transactions sorted by ascending time.
    /// </summary>
    public IEnumerable<TransactionDto> Transactions { get; set; }

    /// <summary>
    ///     Available prices of the instrument represented by the position, sorted by ascending time.
    /// </summary>
    public IEnumerable<InstrumentPriceDto> Prices { get; set; }
}