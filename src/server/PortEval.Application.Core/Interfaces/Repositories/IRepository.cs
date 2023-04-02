namespace PortEval.Application.Core.Interfaces.Repositories
{
    /// <summary>
    /// Represents a persistently stored collection of aggregate roots.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// A unit of work representing a single transaction, which will either be completely committed, or completely rejected.
        /// </summary>
        public IUnitOfWork UnitOfWork { get; }
    }
}
