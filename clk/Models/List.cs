using System;

namespace clk.Models
{
    public class List
    {
        public string id { get; }
        public string name { get; }
        public string boardId { get; }
        public string created { get; }    

        public List(string id, string name, string boardId, string created)
        {
            this.id = id;
            this.name = name;
            this.boardId = boardId;
            this.created = created;
        }

        public override string ToString()
        {
            return $"{nameof(id)}: {id}, {nameof(name)}: {name}, {nameof(boardId)}: {boardId}, {nameof(created)}: {created}";
        }
    }
}