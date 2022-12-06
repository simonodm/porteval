using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Services.Interfaces.Repositories
{
    public interface IInstrumentPriceRepository : IRepository
    {
        public Task<IEnumerable<InstrumentPrice>> ListInstrumentPricesAsync(int instrumentId);
        public Task<InstrumentPrice> FindPriceByIdAsync(int instrumentId, int priceId);
        public Task<InstrumentPrice> FindPriceAtAsync(int instrumentId, DateTime time);
        public InstrumentPrice Add(InstrumentPrice price);
        public Task BulkInsertAsync(IList<InstrumentPrice> prices);
        public Task DeleteAsync(int instrumentId, int priceId);
        public void Delete(InstrumentPrice price);
        public Task<bool> ExistsAsync(int instrumentId, int priceId);
        public Task<bool> ExistsAsync(int instrumentId, DateTime time);
    }
}
