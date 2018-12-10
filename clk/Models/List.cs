using System;

namespace clk.Models
{
    public class List
    {
        public string id { get; }
        public string name { get; }
        public string boardId { get; }

        public List(string id, string name, string boardId)
        {
            this.id = id;
            this.name = name;
            this.boardId = boardId;
        }
    }
}