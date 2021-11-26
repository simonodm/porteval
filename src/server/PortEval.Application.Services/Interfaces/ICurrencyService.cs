using PortEval.Application.Models.DTOs;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces
{
    /// <summary>
    /// Handles currency configuration operations.
    /// </summary>
    public interface ICurrencyService
    {
        /// <summary>
        /// Updates a currency based on supplied options. Only affects the <c>IsDefault</c> field.
        /// </summary>
        /// <param name="options">DTO of currency to update.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        public Task UpdateAsync(CurrencyDto options);
    }
}
