using System.Collections.Generic;
using System.Linq;
using clk.Models;
using clk.Resources;
using Random = clk.Resources.Random;

namespace clk.Controllers
{
    public class OverviewController
    {
        private List<Board> boards;
        private string jsonFile = "boards.json";
        private Json json;

        /// <summary>
        /// Constructor.
        /// Reads all the boards and initialises a json model
        /// </summary>
        public OverviewController()
        {
            json = new Json(jsonFile);
            boards = json.readFile<Board>().Where(x => x.active).ToList();
        }

        /// <summary>
        /// This will return all the active boards.
        /// </summary>
        /// <returns>A List of Board</returns>
        public List<Board> getBoards()
        {
            return boards.Where(x => x.active).ToList();
        }

        /// <summary>
        /// This will create new boards to the Json file.
        /// It will create a unique ID and timestamp for the board,
        /// just provide a name for the new board.
        /// </summary>
        /// <param name="boardName">The name of the new board.</param>
        public string createBoard(string boardName)
        {
            string created = Time.timestamp();
            string id = Random.guid();

            Board board = new Board(id, boardName, created);
            boards.Add(board);
            json.writeFile(boards);

            return id;
        }

        /// <summary>
        /// This will create an already exisiting board (from db ie)
        /// </summary>
        /// <param name="name">The name of the board</param>
        /// <param name="id">The ID of the board</param>
        /// <param name="created">The timestamp of creation</param>
        public void createBoard(string name, string id, string created)
        {
            if (boards.Any(x => x.id == id))
                boards.RemoveAll(x => x.id == id);

            Board b = new Board(id, name, created);
            boards.Add(b);
            json.writeFile(boards);
        }

        /// <summary>
        /// Since the only thing you're able to change on a board
        /// is the name, just the name and the board ID is needed.
        /// </summary>
        /// <param name="boardName">The new name of the board</param>
        /// <param name="boardId">The ID of the board, to change the name of</param>
        public void updateBoard(string boardName, string boardId)
        {
            boards.Find(x => x.id == boardId).name = boardName;
            json.writeFile(boards);
        }

        /// <summary>
        /// This will set a board to inactive (delete)
        /// </summary>
        /// <param name="boardId">The ID of the board to archive.</param>
        public void deleteBoard(string boardId)
        {
            boards.Find(x => x.id == boardId).active = false;
            json.writeFile(boards);
        }
    }
}