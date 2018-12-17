using System.Collections.Generic;
using clk.Models;

namespace clk.Controllers
{
    public class BoardController
    {
        private string boardId;

        public string name { get; set; }
        public string created { get; set; }
        public string id { get; set; }

        public List<List> lists { get; set; }
        public List<Card> cards { get; set; }
        public List<Checklist> checklists { get; set; }
        public List<ChecklistPoint> points { get; set; }
        public List<Comment> comments { get; set; }

        public string userId { get; set; }
        public string password { get; set; }

        private Json boardJson;
        private Json listJson;
        private Json cardJson;
        private Json checkJson;
        private Json pointJson;
        private Json commentJson;

        public BoardController(string boardId)
        {
            this.boardId = boardId;

            boardJson = new Json("boards.json");
            listJson = new Json("lists.json");
            cardJson = new Json("cards.json");
            checkJson = new Json("checklists.json");
            pointJson = new Json("points.json");
            commentJson = new Json("comments.json");

            lists = new List<List>();
            cards = new List<Card>();
            checklists = new List<Checklist>();
            comments = new List<Comment>();
            points = new List<ChecklistPoint>();

            password = "";
            userId = "";

            Board b = boardJson.readFile<Board>().Find(x => x.id == boardId);
            name = b.name;
            created = b.created;

            // Start a chain-reaction and populte all our lists
            populateLists();
        }

        /// <summary>
        /// This will populate the lists list
        /// by finding all lists associated to the board id.
        /// </summary>
        private void populateLists()
        {
            lists = listJson.readFile<List>().FindAll(x => x.boardId == boardId);
            populateCards();
        }

        /// <summary>
        /// This will populate the cards list
        /// by looping through each list and finding associated
        /// cards to each list.
        /// </summary>
        private void populateCards()
        {
            foreach (List list in lists)
            {
                cards.AddRange(cardJson.readFile<Card>().FindAll(x => x.listId == list.id));
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
            foreach (Card card in cards)
            {
                checklists.AddRange(checkJson.readFile<Checklist>().FindAll(x => x.cardId == card.id));
                comments.AddRange(commentJson.readFile<Comment>().FindAll(x => x.cardId == card.id));
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
            foreach (Checklist checklist in checklists)
            {
                points.AddRange(pointJson.readFile<ChecklistPoint>().FindAll(x => x.checklistId == checklist.id));
            }
        }
    }
}