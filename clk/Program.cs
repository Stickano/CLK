using System;
using System.Collections.Generic;
using System.Linq;
using clk.Controllers;
using clk.Models;
using clk.Resources;

namespace clk
{
    internal class Program
    {
        //private static Dictionary<string, string> parameters = new Dictionary<string, string>();
        private static List<Argument> argList = new List<Argument>();
        private static bool boardOverview;
        private static int board = -1;
        private static int list = -1;
        private static int card = -1;

        public static void Main(string[] args)
        {
            sortArgs(args);
            Json.isFiles();

            var argGroups = argList.ToLookup(x => x.key, x => x.value);

            // Loop through all the parameters (args),
            // and perform the action associated with that parameter.
            foreach (IGrouping<string, string> keyVal in argGroups)
            {
                if (keyVal.Key.Equals("-h"))
                    About.usage();
                if (keyVal.Key.Equals("-b") && boardOverview)
                    getBoards();
                if (keyVal.Key.Equals("--new-board"))
                    createBoard(keyVal);
                if (keyVal.Key.Equals("--new-list"))
                    createList(keyVal);
                
            }
                

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
                
                // Stupid workaround, bc I can get it working if the -b has a value with ToLookup IGrouping
                if (keyVal[0].Equals("-b") && val.Equals(""))
                    boardOverview = true;
                
                // Will set global variables, so we know which board, list or card we are working with.
                if (keyVal[0].Equals("-b") && Validators.isInt(val))
                    int.TryParse(val, out board);
                if (keyVal[0].Equals("-l") && Validators.isInt(val))
                    int.TryParse(val, out list);
                if (keyVal[0].Equals("-c") && Validators.isInt(val))
                    int.TryParse(val, out card);

                Argument argument = new Argument {key = keyVal[0], value = val};
                argList.Add(argument);
                //parameters.Add(keyVal[0], val);
            }
        }

        /// <summary>
        /// If --new-board parameter is incl. this is run.
        /// It will invoke the OverviewController (boards).
        /// The controller will handle the heavy work. 
        /// </summary>
        private static void createBoard(IGrouping<string, string> keyVal)
        {   
            OverviewController controller = new OverviewController();
            foreach (var val in keyVal)
            {
                if (val.Equals(""))
                    continue;
                
                controller.createBoard(val);
                Console.WriteLine("Created board: " + val);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// If --new-list parameter is incl. this is run.
        /// It will make sure a board has been selected,
        /// find the ID of that board and initialize a
        /// ListController, which will handle the creation.
        /// </summary>
        /// <param name="keyVal"></param>
        private static void createList(IGrouping<string, string> keyVal)
        {
            if (board < 1)
                return;
            
            OverviewController ovController = new OverviewController();
            string boardId = IDs.getIdFromList(ovController.boards, board - 1);
            ListController controller = new ListController(boardId);
            foreach (var val in keyVal)
            {
                if (val.Equals(""))
                    continue;
                
                controller.createList(val);
                Console.WriteLine("Created list: " + val);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// If -b with no value is incl. this will run.
        /// It will invoke the OverviewController, get the boards,
        /// and print em out for the user.
        /// </summary>
        private static void getBoards()
        {
            OverviewController controller = new OverviewController();
            int br = 0;
            
            Console.WriteLine("Available boards:");
            foreach (Board board in controller.boards)
            {
                br++;
                Console.WriteLine("["+ br +"]: " + board.name);
            }

            Console.WriteLine();
        }
        
    }
}