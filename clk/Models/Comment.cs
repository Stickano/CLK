namespace clk.Models
{
    public class Comment
    {
        public string comment { get; set; }
        public string cardId { get; set; }
        public string id { get; set; }
        public string created { get; set; }
        public bool? active { get; set; } = true;
    }
}