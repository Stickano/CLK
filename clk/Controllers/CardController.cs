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
        public List<Checklist> checklists { get; set; }
        public List<ChecklistPoint> points { get; set; }
        
        private string cardJsonFile = "cards.json";
        private string commentJsonFile = "comments.json";
        private string checklistJsonFile = "checklists.json";
        private string pointJsonFile = "points.json";
        
        private Json cardJson;
        private Json commentJson;
        private Json checklistJson;
        private Json pointJson;

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
            
            checklistJson= new Json(checklistJsonFile);
            checklists = checklistJson.readFile<Checklist>();
            
            pointJson = new Json(pointJsonFile);
            points = pointJson.readFile<ChecklistPoint>();
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
        /// This will create a checklist for a card.
        /// </summary>
        /// <param name="name">The name of the checklist</param>
        /// <param name="cardId">The card ID that holds the checklist</param>
        public string createChecklist(string name, string cardId)
        {
            string created = Time.timestamp();
            string id = Random.guid();
            
            checklists.Add(new Checklist(id, name, cardId, created));
            checklistJson.writeFile(checklists);

            return id;
        }
        
        /// <summary>
        /// This will create a point in a checklist.
        /// </summary>
        /// <param name="description">The description (name) of the point</param>
        /// <param name="checklistId">The checklist ID that holds the point</param>
        public void createChecklistPoint(string description, string checklistId)
        {
            string created = Time.timestamp();
            string id = Random.guid();
            
            points.Add(new ChecklistPoint(description, id, checklistId, created));
            pointJson.writeFile(points);
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
            return comments.FindAll(x => x.cardId == cardId).Where(x => x.active).ToList();
        }

        /// <summary>
        /// This will return all the checklists associated with a card.
        /// </summary>
        /// <param name="cardId">The card ID to collect checklists from</param>
        /// <returns>List of Checklist</returns>
        public List<Checklist> getChecklists(string cardId)
        {
            return checklists.FindAll(x => x.cardId == cardId).Where(x => x.active).ToList();
        }

        /// <summary>
        /// This will return all the points associated with a checklist.
        /// </summary>
        /// <param name="checklistId">The checklist ID that holds the point(s)</param>
        /// <returns>List of ChecklistPoint</returns>
        public List<ChecklistPoint> getChecklistPoints(string checklistId = "")
        {
            if (checklistId.Equals(""))
                return points;
            
            return points.FindAll(x => x.checklistId == checklistId).Where(x => x.active).ToList();
        }

        /// <summary>
        /// This will return all the points found in a card -
        /// It will loop through all the checklists and gather those points,
        /// and return a complete list of Points in a card.
        /// </summary>
        /// <param name="cardId">The ID of the card to search through.</param>
        /// <returns>A List of ChecklistPoint associated to the card.</returns>
        public List<ChecklistPoint> getChecklistPointsInCard(string cardId)
        {
            
            List<ChecklistPoint> cardPoints = new List<ChecklistPoint>();

            foreach (Checklist cl in getChecklists(cardId))
            {
                if (cl.active)
                    cardPoints.AddRange(getChecklistPoints().FindAll(x => x.checklistId == cl.id));
            }

            return cardPoints;
        }

        /// <summary>
        /// This will check/uncheck a point in a checklist.
        /// </summary>
        /// <param name="checklistPointId">The ID of the point to check/uncheck</param>
        public void clickPoint(string checklistPointId)
        {
            var p = points.FindIndex(x => x.id == checklistPointId);
            points[p].isCheck = !points[p].isCheck;
            
            pointJson.writeFile(points);
        }

        /// <summary>
        /// Update a label for a card.
        /// </summary>
        /// <param name="cardId">The ID of the card to update label</param>
        /// <param name="label">The label value</param>
        public void updateLabel(string cardId, Label label)
        {
            cards.Find(x => x.id == cardId).label = label;
            cardJson.writeFile(cards);
        }

        /// <summary>
        /// This will update the name of a card.
        /// </summary>
        /// <param name="name">The new name of the card</param>
        /// <param name="cardId">The ID of the card, to change the name for</param>
        public void updateCard(string name, string cardId)
        {
            cards.Find(x => x.id == cardId).name = name;
            cardJson.writeFile(cards);
        }

        /// <summary>
        /// This will update the name of a checklist.
        /// </summary>
        /// <param name="name">The new name of the checklist</param>
        /// <param name="checklistId">The ID of the checklist to change the name of</param>
        public void updateChecklist(string name, string checklistId)
        {
            checklists.Find(x => x.id == checklistId).name = name;
            checklistJson.writeFile(checklists);
        }

        /// <summary>
        /// Set a comment to inactive (delete).
        /// </summary>
        /// <param name="commentId">The ID of the comment to archive</param>
        public void deleteComment(string commentId)
        {
            comments.Find(x => x.id == commentId).active = false;
            commentJson.writeFile(comments);
        }

        /// <summary>
        /// Set a checklist to inactive.
        /// </summary>
        /// <param name="checkId">The ID of the checklist to archive</param>
        public void deleteChecklist(string checkId)
        {
            checklists.Find(x => x.id == checkId).active = false;
            checklistJson.writeFile(checklists);
        }

        /// <summary>
        /// Set a card to inactive (delete)
        /// </summary>
        /// <param name="cardId">The ID of the card to archive.</param>
        public void deleteCard(string cardId)
        {
            cards.Find(x => x.id == cardId).active = false;
            cardJson.writeFile(cards);
        }

        /// <summary>
        /// Set a checklist point to inactive 
        /// </summary>
        /// <param name="pointId">The ID of the checklist point to archive.</param>
        public void deletePoint(string pointId)
        {
            points.Find(x => x.id == pointId).active = false;
            pointJson.writeFile(points);
        }
    }
}