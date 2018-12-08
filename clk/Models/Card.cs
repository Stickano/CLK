using System;

namespace clk.Models
{
    public class Card
    {
        private string id;
        public String headline { get; set; }
        public String Description { get; set; }

        public Card(string id, string headline, string description)
        {
            this.id = id;
            this.headline = headline;
            Description = description;
        }
    }
}