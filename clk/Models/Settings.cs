using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clk.Models
{
    class Settings
    {
        public string email { get; set; }
        public string password { get; set; }
        public string defaultBoard { get; set; }
        public bool autoSaveToCloud { get; set; }

        public Settings()
        {
            email = null;
            password = null;
            defaultBoard = null;
            autoSaveToCloud = false;
        }
    }
}
