using System.Collections.Generic;
using System.Linq;
using clk.Controllers;
using clk.Resources;


namespace clk.Models
{
    public class TrelloBoard
    {
        public string id { get; set; }
        public string name { get; set; }
        public string created { get; set; }

        private string restResponse;
        private OverviewController oc;
        private ListController lc;
        private CardController cc;

        private string timestamp;

        public TrelloBoard(string restResponse)
        {
            oc = new OverviewController();
            cc = new CardController();

            id = "";
            name = "";
            created = "";

            timestamp = Time.timestamp();
            
            this.restResponse = restResponse;
            setBoard();
        }

        private void setBoard()
        {
            if (restResponse.Equals(""))
                return;
            
            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(restResponse);

            List<dynamic> actions = result["actions"];
            actions.Reverse();
            
            // Mandatory board values
            name = result["name"];
            id = result["id"];
            created = actions.First()["date"];
            created.Substring(10);

            // Create the lists
            lc = new ListController(id);
            foreach (dynamic list in result["lists"])
            {
                string listId = list["id"];
                string listName = list["name"];
                lc.createList(listName, timestamp, listId);
            }
            
            // Create the cards
            foreach (dynamic card in result["cards"])
            {
                string cardDesc = card["desc"];
                string cardName = card["name"];
                string cardId = card["id"];
                cc.createCard(cardName, timestamp, cardId, cardDesc);
            }
            
            // Create checklists & points
            foreach (dynamic check in result["checklists"])
            {

                string checkName = check["name"];
                string checkId = check["id"];
                string checkCard = check["idCard"];
                cc.createChecklist(checkName, checkCard, timestamp, checkId);
                
                // points
                foreach (dynamic point in check["checkItems"])
                {
                    bool completed = false;
                    string state = point["state"];
                    if (state.Equals("complete"))
                        completed = true;

                    string pointName = point["name"];
                    string pointId = point["id"];

                    cc.createChecklistPoint(pointName, checkId, pointId, timestamp, completed);
                }
            }
            
            // Create comments
            foreach (dynamic comment in result["actions"])
            {
                string type = comment["type"];
                if(!type.Equals("commentCard"))
                    continue;

                string comTxt = comment["text"];
                string comCard = comment["card"]["id"];
                
                cc.createComment(comTxt,comCard);
            }
            
        }
    }
}