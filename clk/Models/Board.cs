namespace clk.Models
{
    public class Board
    {
        private string id;   
        public string name { get; set; }

        public Board(string id, string name)
        {
            this.id= id;
            this.name = name;
        }
    }
}