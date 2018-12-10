using System;

namespace clk.Models
{
    public class Card
    {
        public string id { get; }
        public string name { get; }
        public string created { get;}
        public string listId { get; }
        public string description { get; set; }

        public Card(string id, string name, string created, string listId)
        {
            this.id = id;
            this.name = name;
            this.created = created;
            this.listId = listId;
        }
    }
}