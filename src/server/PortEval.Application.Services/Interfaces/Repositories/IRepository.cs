namespace PortEval.Application.Services.Interfaces.Repositories
{
    /// <summary>
    /// Represents a persistently stored collection of aggregate roots.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// A single unit of work.
        /// </summary>
        public IUnitOfWork UnitOfWork { get; }
    }
}
