using System.Collections.Generic;
using System.Linq;
using clk.Models;
using clk.Resources;

namespace clk.Controllers
{
    public class CardController
    {
        public string listId { get; }
        public List<Card> cards { get; }
        private string jsonFile = "cards.json";
        private Json json;

        public CardController(string listId)
        {
            this.listId = listId;
            json = new Json(jsonFile);
            cards = json.readFile<Card>();
        }

        /// <summary>
        /// Create a new card.
        /// </summary>
        /// <param name="cardName">The name of the new card</param>
        public void createCard(string cardName, string description="")
        {
            string created = Time.timestamp();
            string id = Random.guid();

            Card card = new Card(id, cardName, created, listId);
            if (!description.Equals(""))
                card.description = description;
            
            cards.Add(card);
            
            json.writeFile(cards);
        }

        /// <summary>
        /// Set the description for a card. 
        /// </summary>
        /// <param name="description">The description for a specific card</param>
        /// <param name="cardId">The card id, singling out he specific card</param>
        public void createDescription(string description, string cardId)
        {
            cards.Find(x => x.id == cardId).description = description;
            json.writeFile(cards);
        }

        /// <summary>
        /// This will return a sorted list of cards -
        /// Only the ones related to the list id,
        /// instead of all cards.
        /// </summary>
        /// <returns>List of Card, only for specific list (id)</returns>
        public List<Card> getCards()
        {
            var cardCriteria = from l in cards where l.listId == listId select l;
            List<Card> sortedLists = cardCriteria.ToList();
            return sortedLists;
        }
    }
}