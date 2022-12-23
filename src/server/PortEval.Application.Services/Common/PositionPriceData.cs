using System;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;

namespace PortEval.Application.Features.Common
{
    /// <summary>
    /// Represents a collection of position's transactions and the corresponding instrument's price at a given time.
    /// </summary>
    public class PositionPriceData
    {
        /// <summary>
        /// Position's transactions.
        /// </summary>
        public IEnumerable<TransactionDto> Transactions { get; set; }

        /// <summary>
        /// Time of <see cref="Price"/>
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Position's instrument price at <see cref="Time"/>.
        /// </summary>
        public InstrumentPriceDto Price { get; set; }
    }
}
