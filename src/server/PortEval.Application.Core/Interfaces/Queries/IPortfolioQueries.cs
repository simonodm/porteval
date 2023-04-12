﻿using PortEval.Application.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Queries
{
    public interface IPortfolioQueries
    {
        Task<IEnumerable<PortfolioDto>> GetAllPortfoliosAsync();
        Task<PortfolioDto> GetPortfolioAsync(int portfolioId);
    }
}