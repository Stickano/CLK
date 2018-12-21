
namespace clk.Models
{
    public class Board
    {
        public string id { get; }   
        public string name { get; set; }
        public string created { get; }
        public bool active { get; set; }

        public Board(string id, string name, string created, bool active = true)
        {
            this.id = id;
            this.name = name;
            this.created = created;
            this.active = active;
        }

        public override string ToString()
        {
            return $"{nameof(id)}: {id}, {nameof(name)}: {name}, {nameof(created)}: {created}";
        }
    }
}