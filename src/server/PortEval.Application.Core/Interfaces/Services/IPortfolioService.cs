using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles creation, modification and removal of portfolios.
    /// </summary>
    public interface IPortfolioService
    {
        /// <summary>
        /// Creates a portfolio according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous creation operation. Task result contains newly created portfolio entity.</returns>
        Task<Portfolio> CreatePortfolioAsync(PortfolioDto options);

        /// <summary>
        /// Modifies a portfolio according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous update operation. Task result contains the updated portfolio entity.</returns>
        Task<Portfolio> UpdatePortfolioAsync(PortfolioDto options);

        /// <summary>
        /// Deletes a portfolio.
        /// </summary>
        /// <param name="id">Portfolio ID.</param>
        /// <returns>A task representing the asynchronous deletion operation.</returns>
        Task DeletePortfolioAsync(int id);
    }
}
