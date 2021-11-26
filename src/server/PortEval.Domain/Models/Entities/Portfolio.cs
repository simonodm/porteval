namespace PortEval.Domain.Models.Entities
{
    public class Portfolio : VersionedEntity, IAggregateRoot
    {
        public int Id { get; private set; }
        public string CurrencyCode { get; private set; }
        public string Name { get; private set; }
        public string Note { get; private set; }

        public Portfolio(string name, string note, string currencyCode)
        {
            Name = name;
            Note = note;
            CurrencyCode = currencyCode;
        }

        public void Rename(string name)
        {
            Name = name;
        }

        public void SetNote(string note)
        {
            Note = note;
        }

        public void ChangeCurrency(string currencyCode)
        {
            CurrencyCode = currencyCode;
        }
    }
}
