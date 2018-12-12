namespace clk.Models
{
    public class ChecklistPoint
    {
        public string name { get; }
        public string id { get; }
        public string checklistId { get; }
        public string created { get; }

        public ChecklistPoint(string name, string id, string checklistId, string created)
        {
            this.name = name;
            this.id = id;
            this.checklistId = checklistId;
            this.created = created;
        }

        public override string ToString()
        {
            return $"{nameof(name)}: {name}, {nameof(id)}: {id}, {nameof(checklistId)}: {checklistId}, {nameof(created)}: {created}";
        }
    }
}