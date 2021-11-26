using PortEval.Application.Models.DTOs;
using PortEval.Domain.Models.Entities;
using System.Threading.Tasks;

namespace PortEval.Application.Services.Interfaces
{
    /// <summary>
    /// Handles creation, modification and removal of positions.
    /// </summary>
    public interface IPositionService
    {
        /// <summary>
        /// Creates a position according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous creation operation. Task result contains newly created position entity.</returns>
        public Task<Position> AddPositionAsync(PositionDto options);

        /// <summary>
        /// Modifies a position according to the supplied DTO.
        /// </summary>
        /// <param name="options">A DTO containing client's request body.</param>
        /// <returns>A task representing the asynchronous update operation. Task result contains the updated position entity.</returns>
        public Task<Position> UpdatePositionAsync(PositionDto options);

        /// <summary>
        /// Deletes a position.
        /// </summary>
        /// <param name="portfolioId">Parent portfolio ID.</param>
        /// <param name="positionId">Position ID.</param>
        /// <returns>A task representing the asynchronous deletion operation.</returns>
        public Task RemovePositionAsync(int positionId);
    }
}
