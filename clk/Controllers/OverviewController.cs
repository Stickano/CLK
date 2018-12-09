using System;
using System.Collections.Generic;
using System.Linq;
using clk.Models;
using clk.Resources;
using Newtonsoft.Json;
using Random = clk.Resources.Random;

namespace clk.Controllers
{
    public class OverviewController
    {
        private string jsonFile = "boards.json";
        private List<Board> boards;
        private Json json;

        public OverviewController()
        {
            boards = new List<Board>();
            fetchBoards();
        }

        /// <summary>
        /// This will be fetch all the stored boards,
        /// from the Json file, and populate the boards List.
        /// </summary>
        private void fetchBoards()
        {
            json = new Json(jsonFile);

            if (json.readFile() == null)
                return;

            //List<Board> b = json.readFile().Cast<Board>().ToList();
            List<object> fromJson = json.readFile();
            List<Board> toBoards = fromJson.ConvertAll(x => (Board) x);
            foreach (Board board in toBoards)
            {
                Console.WriteLine(board.id);
            }
            
            /*foreach (Board board in json.readFile())
            {
                Console.WriteLine(board.ToString());
                //IEnumerable<Board> a = board.ca
                //Board b = new Board(a.id, a.name, a.created);
                boards.Add(board);
            }*/
        }

        public List<Board> getBoards()
        {
            return boards;
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
            
            Json json = new Json(jsonFile);
            json.writeFile(boards.Cast<object>().ToList());
        }
    }
}