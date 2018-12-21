using System;

namespace clk.Models
{
    public class Card
    {
        // Cards can (optional) hold 1 description and 1 label.
        private string _description;
        private Label _label;
        
        // Required parameters of a card
        public string id { get; }
        public string name { get; set; }
        public string created { get;}
        public string listId { get; }
        public bool active { get; set; }

        // Set/get an color for the card
        public Label label
        {
            get { return _label; }
            set { _label = value; }
        }

        // Set/get a description for the card
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Constructor.
        /// Initialize a card with the required data stored about it.
        /// </summary>
        /// <param name="id">The ID of the card</param>
        /// <param name="name">The name of the card</param>
        /// <param name="created">Timestamp on creation</param>
        /// <param name="listId">The ID of the list,that this card resides in.</param>
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