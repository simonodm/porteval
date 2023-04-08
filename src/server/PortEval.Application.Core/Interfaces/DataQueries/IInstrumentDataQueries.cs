using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.QueryParams;
using PortEval.Domain.Models.Enums;

namespace PortEval.Application.Core.Interfaces.DataQueries
{
    public interface IInstrumentDataQueries
    {
        Task<IEnumerable<InstrumentDto>> GetAllInstrumentsAsync();
        Task<int> GetInstrumentCountAsync();
        Task<IEnumerable<InstrumentDto>> GetInstrumentPageAsync(PaginationParams pagination);
        Task<InstrumentDto> GetInstrumentAsync(int instrumentId);

        Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesAsync(int instrumentId, DateTime from,
            DateTime to);

        Task<int> GetInstrumentPriceCountAsync(int instrumentId, DateTime from, DateTime to,
            AggregationFrequency? frequency);

        Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesPage(
            int instrumentId,
            DateTime from,
            DateTime to,
            PaginationParams pagination,
            AggregationFrequency? frequency);

        Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPriceCompressedCount(int instrumentId,
            DateTime from, DateTime to,
            AggregationFrequency? frequency);

        Task<IEnumerable<InstrumentPriceDto>> GetInstrumentPricesPageCompressed(int instrumentId,
            DateTime from, DateTime to,
            PaginationParams pagination, AggregationFrequency? frequency);

        Task<InstrumentPriceDto> GetInstrumentPriceAsync(int instrumentId, DateTime time);
        Task<IEnumerable<InstrumentSplitDto>> GetInstrumentSplitsAsync(int instrumentId);
        Task<InstrumentSplitDto> GetInstrumentSplitAsync(int instrumentId, int splitId);
    }
}