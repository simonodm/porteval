using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Queries.Interfaces
{
    public interface IExchangeQueries
    {
        public Task<QueryResponse<IEnumerable<ExchangeDto>>> GetKnownExchanges();
    }
}
