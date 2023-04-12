using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Queries
{
    public interface IInstrumentQueries
    {
        Task<IEnumerable<InstrumentDto>> GetAllInstrumentsAsync();
        Task<int> GetInstrumentCountAsync();
        Task<IEnumerable<InstrumentDto>> GetInstrumentPageAsync(PaginationParams pagination);
        Task<InstrumentDto> GetInstrumentAsync(int instrumentId);

        Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesAsync(int instrumentId, DateTime from,
            DateTime to);

        Task<int> GetInstrumentPriceCountAsync(int instrumentId, DateTime from, DateTime to,
            AggregationFrequency? frequency = null);

        Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesPageAsync(
            int instrumentId,
            DateTime from,
            DateTime to,
            PaginationParams pagination,
            AggregationFrequency? frequency = null);

        Task<int> GetInstrumentPriceCompressedCountAsync(int instrumentId,
            DateTime from, DateTime to,
            AggregationFrequency? frequency = null);

        Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesPageCompressedAsync(int instrumentId,
            DateTime from, DateTime to,
            PaginationParams pagination, AggregationFrequency? frequency = null);

        Task<InstrumentPriceDto> GetInstrumentPriceAsync(int instrumentId, DateTime time);
        Task<IEnumerable<InstrumentSplitDto>> GetInstrumentSplitsAsync(int instrumentId);
        Task<InstrumentSplitDto> GetInstrumentSplitAsync(int instrumentId, int splitId);
    }
}