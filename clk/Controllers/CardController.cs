using System.Collections.Generic;
using clk.Models;
using clk.Resources;

namespace clk.Controllers
{
    public class CardController
    {
        public string boardId { get; }
        public string listId { get; }
        public List<Card> cards { get; }
        private string jsonFile = "cards.json";
        private Json json;

        public CardController(string boardId, string listId)
        {
            this.boardId = boardId;
            this.listId = listId;
            json = new Json(jsonFile);
            cards = json.readFile<Card>();
        }

        public void createCard(string cardName)
        {
            string created = Time.timestamp();
            string id = Random.guid();

            Card card = new Card(id, cardName, created, listId);
            cards.Add(card);
            
            Json json = new Json(jsonFile);
            json.writeFile(cards);
        }
    }
}