using System;

namespace clk.Models
{
    public class Card
    {
        private string _description;
        public string id { get; }
        public string name { get; }
        public string created { get;}
        public string listId { get; }

        public string description
        {
            get { return _description; }
            set { _description = value; }
        }

        public Card(string id, string name, string created, string listId)
        {
            this.id = id;
            this.name = name;
            this.created = created;
            this.listId = listId;
            _description = "";
        }

        public override string ToString()
        {
            return $"{nameof(id)}: {id}, {nameof(name)}: {name}, {nameof(created)}: {created}, {nameof(listId)}: {listId}, {nameof(description)}: {description}";
        }
    }
}