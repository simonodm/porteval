using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Repositories;

/// <summary>
///     Represents a single unit of work, wrapping a single persistence layer transaction.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    ///     Persists the changes.
    /// </summary>
    public void Commit();

    /// <inheritdoc cref="IUnitOfWork.Commit" />
    /// <returns>A task representing the asynchronous persistence operation.</returns>
    public Task CommitAsync();
}