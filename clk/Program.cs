using System;
using System.Collections.Generic;
using clk.Resources;

namespace clk
{
    internal class Program
    {
        private static Dictionary<string, string> parameters = new Dictionary<string, string>();
        
        public static void Main(string[] args)
        {
            sortArgs(args);

            if (parameters.ContainsKey("-h") || parameters.ContainsKey("--help"))
                About.Usage();

        }
        
        /// <summary>
        /// This will sort the init parameters into a global dictionary
        /// of parameter => value
        /// </summary>
        /// <param name="args">The arguments as received from Main(args)</param>
        private static void sortArgs(string[] args)
        {
            // Order the parameters as key => value 
            foreach (string arg in args)
            {
                string[] keyVal = arg.Split('=');
                
                string val = "";
                if (keyVal.Length > 1)
                    val = keyVal[1].ToLower();
                
                parameters.Add(keyVal[0].ToLower(), val);
            }
        }
    }
}