namespace PortEval.Domain.Models.Entities
{
    public class Exchange : VersionedEntity, IAggregateRoot
    {
        public string Symbol { get; private set; }
        public string Name { get; private set; }

        internal Exchange(string symbol)
        {
            Symbol = symbol;
        }

        internal Exchange(string symbol, string name)
        {
            Symbol = symbol;
            Name = name;
        }

        public static Exchange Create(string symbol)
        {
            return new Exchange(symbol);
        }

        public static Exchange Create(string symbol, string name)
        {
            return new Exchange(symbol, name);
        }

        public void Rename(string newName)
        {
            Name = newName;
        }
    }
}
