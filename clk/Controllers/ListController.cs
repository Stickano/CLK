using System.Collections.Generic;
using System.Linq;
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
            json = new Json(jsonFile);
            lists = json.readFile<List>();
        }

        /// <summary>
        /// This will create a new list to the Json file.
        /// </summary>
        /// <param name="listName">The name of the new list</param>
        public void createList(string listName)
        {
            string created = Time.timestamp();
            string id = Random.guid();

            List list = new List(id, listName, boardId, created);
            lists.Add(list);
            
            Json json = new Json(jsonFile);
            json.writeFile(lists);
        }

        /// <summary>
        /// This will return all the lists available for the
        /// specific board, that this class has been initialized with.
        /// </summary>
        /// <returns>List of lists (Kanban lists) associated with given board</returns>
        public List<List> getLists()
        {
            var boardCriteria = from l in lists where l.boardId == boardId select l;
            List<List> sortedLists = boardCriteria.ToList();
            return sortedLists;
        }
    }
}