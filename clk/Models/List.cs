using System;

namespace clk.Models
{
    public class List
    {
        private string id;
        public String name { get; set; }

        public List(string id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}