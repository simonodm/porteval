using System;
using System.Threading.Tasks;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Services.Interfaces.Repositories
{
    public interface IInstrumentPriceRepository : IRepository
    {
        public Task<InstrumentPrice> FindPriceAt(int instrumentId, DateTime time);
        public InstrumentPrice AddInstrumentPrice(InstrumentPrice price);
        public Task DeleteInstrumentPriceAsync(int instrumentId, int priceId);
    }
}
