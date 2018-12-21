namespace clk.Models
{
    public class Checklist
    {
        public string id { get; }
        public string name { get; set; }
        public string cardId { get;  }
        public string created { get;  }
        public bool active { get; set; }

        public Checklist(string id, string name, string cardId, string created)
        {
            this.id = id;
            this.name = name;
            this.cardId = cardId;
            this.created = created;
        }

        public override string ToString()
        {
            return $"{nameof(id)}: {id}, {nameof(name)}: {name}, {nameof(cardId)}: {cardId}, {nameof(created)}: {created}";
        }
    }
}