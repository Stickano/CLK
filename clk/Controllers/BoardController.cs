using System.Collections.Generic;
using clk.Models;

namespace clk.Controllers
{
    public class BoardController
    {
        private string boardId;

        public List<List> Lists { get; set; }
        public List<Card> Cards { get; set; }
        public List<Checklist> Checklists { get; set; }
        public List<ChecklistPoint> ChecklistPoints { get; set; }
        public List<Comment> Comments { get; set; }

        private Json listJson;
        private Json cardJson;
        private Json checkJson;
        private Json pointJson;
        private Json commentJson;

        public BoardController(string boardId)
        {
            this.boardId = boardId;

            listJson = new Json("lists.json");
            cardJson = new Json("cards.json");
            checkJson = new Json("checklists.json");
            pointJson = new Json("points.json");
            commentJson = new Json("comments.json");

            // Start a chain-reaction and populte all our lists
            populateLists();
        }

        /// <summary>
        /// This will populate the lists list
        /// by finding all lists associated to the board id.
        /// </summary>
        private void populateLists()
        {
            Lists = listJson.readFile<List>().FindAll(x => x.boardId == boardId);
            populateCards();
        }

        /// <summary>
        /// This will populate the cards list
        /// by looping through each list and finding associated
        /// cards to each list.
        /// </summary>
        private void populateCards()
        {
            foreach (List list in Lists)
            {
                Cards.AddRange(cardJson.readFile<Card>().FindAll(x => x.listId == list.id));
            }

            populateChecklistsAndComments();
        }

        /// <summary>
        /// This will populate both checklists and comments lists
        /// by looping through each card and finding associated 
        /// checklists and comments to each card.
        /// </summary>
        private void populateChecklistsAndComments()
        {
            foreach (Card card in Cards)
            {
                Checklists.AddRange(checkJson.readFile<Checklist>().FindAll(x => x.cardId == card.id));
                Comments.AddRange(commentJson.readFile<Comment>().FindAll(x => x.cardId == card.id));
            }

            populatePoints();
        }

        /// <summary>
        /// This will populate the checklist points lists
        /// by loop through all checklists and finding associated
        /// points to each checklist
        /// </summary>
        private void populatePoints()
        {
            foreach (Checklist checklist in Checklists)
            {
                ChecklistPoints.AddRange(pointJson.readFile<ChecklistPoint>().FindAll(x => x.checklistId == checklist.id));
            }
        }
    }
}