namespace PortEval.Domain.Models.Entities
{
    public abstract class VersionedEntity
    {
        public int Version { get; protected set; }
        public void IncreaseVersion()
        {
            Version++;
        }
    }
}
