using clk.Controllers;
using Newtonsoft.Json;

namespace clk.Models
{
    public class Trello
    {
        private OverviewController oc;

        public Trello(OverviewController oc)
        {
            this.oc = oc;
        }
        
        public int importBoard(string url)
        {

            BoardController bc = JsonConvert.DeserializeObject<BoardController>(url);

            return 1;
        }
    }
}