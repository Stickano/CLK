using System.Collections.Generic;

namespace clk.Models
{
    public class Argument
    {
        public string key { get; set; }
        public List<string> value { get; } = new List<string>();
    }
}