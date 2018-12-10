using System.Collections.Generic;
using clk.Models;
using clk.Resources;

namespace clk.Controllers
{
    public class ListController
    {
        public string boardId { get; }
        public List<List> lists { get; }
        private string jsonFile = "lists.json";
        private Json json;

        public ListController(string boardId)
        {
            this.boardId = boardId;
            lists = new List<List>();
            json = new Json(jsonFile);
            lists = json.readFile<List>();
        }

        public void createList(string listName)
        {
            string created = Time.timestamp();
            string id = Random.guid();

            List list = new List(id, listName, boardId);
            lists.Add(list);
            
            Json json = new Json(jsonFile);
            json.writeFile(lists);
        }
    }
}