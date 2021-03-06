namespace clk.Models
{
    public class ChecklistPoint
    {
        public string name { get; set; }
        public string id { get; }
        public string checklistId { get; }
        public string created { get; }
        public bool isCheck { get; set; }
        public bool? active { get; set; }

        public ChecklistPoint(string name, string id, string checklistId, string created, bool isCheck=false, bool active = true)
        {
            this.name = name;
            this.id = id;
            this.checklistId = checklistId;
            this.created = created;
            this.isCheck = isCheck;
            this.active = active;
        }

        public override string ToString()
        {
            return $"{nameof(name)}: {name}, {nameof(id)}: {id}, {nameof(checklistId)}: {checklistId}, {nameof(created)}: {created}";
        }
    }
}