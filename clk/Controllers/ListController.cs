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
            lists = json.readFile<List>().Where(x => x.active == true).ToList();
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
            
            json.writeFile(lists);
        }

        /// <summary>
        /// This will also create a list, but one that is already existing (i.e. from the db).
        /// </summary>
        /// <param name="listName">The name of the list</param>
        /// <param name="created">The date the list was created</param>
        /// <param name="id">The ID of the list</param>
        public void createList(string listName, string created, string id)
        {
            if (lists.Any(x => x.id == id))
                lists.RemoveAll(x => x.id == id);

            List list = new List(id, listName, boardId, created);
            lists.Add(list);
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
            return sortedLists.Where(x => x.active == true).ToList();
        }

        /// <summary>
        /// This will update the name of a list.
        /// </summary>
        /// <param name="name">The new name of the list.</param>
        /// <param name="listId">The ID of the list to update</param>
        public void updateList(string name, string listId)
        {
            lists.Find(x => x.id == listId).name = name;
            json.writeFile(lists);
        }

        /// <summary>
        /// The will delete (set inactive) a list.
        /// </summary>
        /// <param name="listId">The ID of the list to archive.</param>
        public void deleteList(string listId)
        {
            if (lists.All(x => x.id != listId))
                return;

            lists.Find(x => x.id == listId).active = false;
            json.writeFile(lists);
        }
    }
}