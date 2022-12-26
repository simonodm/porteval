using PortEval.Application.Features.Queries;
using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.Queries
{
    public interface IExchangeQueries
    {
        public Task<QueryResponse<IEnumerable<ExchangeDto>>> GetKnownExchanges();
    }
}
