
namespace clk.Models
{
    public class Board
    {
        public string id { get; }   
        public string name { get; }
        public string created { get; }

        public Board(string id, string name, string created)
        {
            this.id = id;
            this.name = name;
            this.created = created;
        }
    }
}