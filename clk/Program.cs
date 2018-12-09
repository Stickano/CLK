using System;
using System.Collections.Generic;
using clk.Controllers;
using clk.Models;
using clk.Resources;

namespace clk
{
    internal class Program
    {
        private static Dictionary<string, string> parameters = new Dictionary<string, string>();
        
        public static void Main(string[] args)
        {
            sortArgs(args);
            Json.isFiles();

            if (parameters.ContainsKey("-h"))
                About.usage();
            if (parameters.ContainsKey("-b") && parameters["-b"].Equals(""))
                getBoards();
            if (parameters.ContainsKey("--new-board"))
                createBoard();
                

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
                // Split our strings into key => value
                string[] keyVal = arg.Split('=');
                
                // Clean the keys a little - 
                // Becomes a benefit elsewhere in the script,
                // because we don't have to match as many values.
                if (keyVal[0].Equals("--help"))
                    keyVal[0] = "-h";
                if (keyVal[0].Equals("--about"))
                    keyVal[0] = "-a";
                if (keyVal[0].Equals("--board"))
                    keyVal[0] = "-b";
                if (keyVal[0].Equals("--list"))
                    keyVal[0] = "-l";
                if (keyVal[0].Equals("--card"))
                    keyVal[0] = "-c";
                
                // Make sure there always is an value (empty if nothing else),
                // so we don't run into nullexceptionref.
                string val = "";
                if (keyVal.Length > 1)
                    val = keyVal[1];
                
                parameters.Add(keyVal[0], val);
            }
        }

        /// <summary>
        /// If --new-board parameter is incl. this is run.
        /// It will invoke the OverviewController (boards).
        /// The controller will handle the heavy work. 
        /// </summary>
        private static void createBoard()
        {
            // Return if no value is given
            if (parameters["--new-board"].Equals(""))
                return;
            
            OverviewController controller = new OverviewController();
            controller.createBoard(parameters["--new-board"]);
        }

        private static void getBoards()
        {
            OverviewController controller = new OverviewController();
            int br = 0;
            foreach (Board board in controller.getBoards())
            {
                Console.WriteLine("["+ br++ +"]: " + board.name);
            }
        }
        
    }
}