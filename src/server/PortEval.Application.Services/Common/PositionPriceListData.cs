﻿using PortEval.Application.Models.DTOs;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    /// <summary>
    /// Represents a collection of a position's transactions and the corresponding instrument's prices.
    /// </summary>
    public class PositionPriceListData
    {
        /// <summary>
        /// Position's transactions.
        /// </summary>
        public IEnumerable<TransactionDto> Transactions { get; set; }

        /// <summary>
        /// Available prices of the instrument represented by the position.
        /// </summary>
        public IEnumerable<InstrumentPriceDto> Prices { get; set; }
    }
}
