using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.Queries
{
    public interface IExchangeQueries
    {
        Task<IEnumerable<ExchangeDto>> GetKnownExchangesAsync();
    }
}