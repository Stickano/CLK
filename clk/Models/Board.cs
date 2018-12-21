
namespace clk.Models
{
    public class Board
    {
        public string id { get; }   
        public string name { get; set; }
        public string created { get; }
        public bool active { get; set; }

        public Board(string id, string name, string created)
        {
            this.id = id;
            this.name = name;
            this.created = created;
        }

        public override string ToString()
        {
            return $"{nameof(id)}: {id}, {nameof(name)}: {name}, {nameof(created)}: {created}";
        }
    }
}