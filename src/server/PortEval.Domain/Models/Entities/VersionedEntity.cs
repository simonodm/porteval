namespace PortEval.Domain.Models.Entities;

/// <summary>
///     Represents a mutable entity which can have different versions.
/// </summary>
public abstract class VersionedEntity : Entity
{
    /// <summary>
    ///     The current version of the entity.
    /// </summary>
    public int Version { get; protected set; }

    /// <summary>
    ///     Increments the entity version.
    /// </summary>
    public void IncreaseVersion()
    {
        Version++;
    }
}