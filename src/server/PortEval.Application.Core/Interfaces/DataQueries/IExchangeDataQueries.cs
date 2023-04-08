using System.Collections.Generic;
using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Interfaces.DataQueries
{
    public interface IExchangeDataQueries
    {
        Task<IEnumerable<ExchangeDto>> GetKnownExchangesAsync();
    }
}