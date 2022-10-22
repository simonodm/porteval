﻿using PortEval.Domain.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces.Repositories
{
    /// <summary>
    /// Represents a persistently stored collection of portfolios.
    /// </summary>
    public interface IPortfolioRepository : IRepository
    {
        /// <summary>
        /// Lists all portfolios.
        /// </summary>
        /// <returns>A task representing the asynchronous retrieval operation. The task result contains an <c>IEnumerable</c> containing all existing portfolios.</returns>
        public Task<IEnumerable<Portfolio>> ListAllAsync();

        /// <summary>
        /// Finds a portfolio by ID.
        /// </summary>
        /// <param name="id">Portfolio ID.</param>
        /// <returns>A task representing the asynchronous search operation. The task result contains the portfolio entity with the supplied ID if it exists, null otherwise.</returns>
        public Task<Portfolio> FindAsync(int id);

        /// <summary>
        /// Adds a portfolio.
        /// </summary>
        /// <param name="portfolio">Portfolio entity to add.</param>
        /// <returns>The added portfolio entity with identity set.</returns>
        public Portfolio Add(Portfolio portfolio);

        /// <summary>
        /// Updates a portfolio.
        /// </summary>
        /// <param name="portfolio">Updated portfolio entity.</param>
        /// <returns>The updated portfolio.</returns>
        public Portfolio Update(Portfolio portfolio);

        /// <summary>
        /// Deletes a portfolio.
        /// </summary>
        /// <param name="portfolioId">If of portfolio to delete.</param>
        /// <returns>A task representing the asynchronous deletion operation.</returns>
        public Task Delete(int portfolioId);

        /// <summary>
        /// Checks whether a portfolio with the supplied portfolio id exists.
        /// </summary>
        /// <param name="id">Portfolio id.</param>
        /// <returns>true if a portfolio with the supplied portfolio id exists in the database, false otherwise</returns>
        public Task<bool> Exists(int id);
    }
}
