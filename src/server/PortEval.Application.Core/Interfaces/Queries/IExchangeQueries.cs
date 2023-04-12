using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Queries
{
    public interface IExchangeQueries
    {
        Task<IEnumerable<ExchangeDto>> GetKnownExchangesAsync();
    }
}