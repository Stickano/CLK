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
        public List<Comment> comments { get; }
        
        private string cardJsonFile = "cards.json";
        private string commentJsonFile = "comments.json";
        
        private Json cardJson;
        private Json commentJson;

        /// <summary>
        /// Constructor.
        /// Will set a Json object for both comments and cards.
        /// (comments are related to cards, so seemed logical at the time)
        /// </summary>
        /// <param name="listId">The ID of the list, to fetch/save card(s) from/to.</param>
        public CardController(string listId)
        {
            this.listId = listId;
            
            cardJson = new Json(cardJsonFile);
            cards = cardJson.readFile<Card>();
            
            commentJson = new Json(commentJsonFile);
            comments = commentJson.readFile<Comment>();
        }

        /// <summary>
        /// Create a new card.
        /// </summary>
        /// <param name="cardName">The name of the new card</param>
        public string createCard(string cardName, string description="")
        {
            string created = Time.timestamp();
            string id = Random.guid();

            Card card = new Card(id, cardName, created, listId);
            if (!description.Equals(""))
                card.description = description;
            
            cards.Add(card);
            cardJson.writeFile(cards);

            return id;
        }

        /// <summary>
        /// Set the description for a card. 
        /// </summary>
        /// <param name="description">The description for a specific card</param>
        /// <param name="cardId">The card id, singling out he specific card</param>
        public void createDescription(string description, string cardId)
        {
            cards.Find(x => x.id == cardId).description = description;
            cardJson.writeFile(cards);
        }

        /// <summary>
        /// This will create a comment for a card.
        /// </summary>
        /// <param name="comment">The comment text</param>
        /// <param name="cardId">The ID for the card to comment on</param>
        public void createComment(string comment, string cardId)
        {
            string created = Time.timestamp();
            string id = Random.guid();
            
            comments.Add(new Comment{cardId = cardId, comment = comment, id = id, created = created});
            commentJson.writeFile(comments);
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

        /// <summary>
        /// This will return all the comments associated with a card.
        /// </summary>
        /// <param name="cardId">The card ID to collect comments from</param>
        /// <returns>List of Comment</returns>
        public List<Comment> getComments(string cardId)
        {
            return comments.FindAll(x => x.cardId == cardId).ToList();
        }
    }
}