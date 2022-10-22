namespace PortEval.Domain.Models.Entities
{
    public class Exchange : VersionedEntity, IAggregateRoot
    {
        public string Symbol { get; private set; }
        public string Name { get; private set; }

        public Exchange(string symbol)
        {
            Symbol = symbol;
        }

        public Exchange(string symbol, string name)
        {
            Symbol = symbol;
            Name = name;
        }

        public void Rename(string newName)
        {
            Name = newName;
        }
    }
}
