using System.Collections.Generic;
using System.Linq;
using clk.Models;
using clk.Resources;
using Random = clk.Resources.Random;

namespace clk.Controllers
{
    public class OverviewController
    {
        public List<Board> boards { get; }
        private string jsonFile = "boards.json";
        private Json json;

        public OverviewController()
        {
            json = new Json(jsonFile);
            boards = json.readFile<Board>();
        }

        /// <summary>
        /// This will create new boards to the Json file.
        /// It will create a unique ID and timestamp for the board,
        /// just provide a name for the new board.
        /// </summary>
        /// <param name="boardName">The name of the new board.</param>
        public void createBoard(string boardName)
        {
            string created = Time.timestamp();
            string id = Random.guid();

            Board board = new Board(id, boardName, created);
            boards.Add(board);
            
            json.writeFile(boards);
        }
    }
}