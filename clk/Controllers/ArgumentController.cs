using System.Collections.Generic;
using System.Linq;
using clk.Models;
using clk.Resources;

namespace clk.Controllers
{
    public class ArgumentController
    {
        public List<Argument> argList { get; }
        private int _board;
        private int _list;
        private int _card;

        public int board
        {
            get { return _board; }
            set { _board = value; }
        }
        public int list
        {
            get { return _list; }
            set { _list = value; }
        }
        public int card
        {
            get { return _card; }
            set { _card = value; }
        }

        public bool boardOverview { get; set; }
        public bool newCard { get; set; }
        public bool newDescription { get; set; }
        public bool newComment { get; set; }

        public ArgumentController(string[] args)
        {
            argList = new List<Argument>();
            board = -1;
            list = -1;
            card = -1;
            
            sortArgs(args);
        }

        /// <summary>
        /// This will return a collection that makes it a whole lot easier to work with the arguments.
        /// It will return a collection of Key (ex -b) => value (ex 2), value etc.
        /// </summary>
        /// <returns>ILookup of string string values (key=>value)</returns>
        public ILookup<string, string> getKeyVal()
        {
            return argList.ToLookup(x => x.key, x => x.value);
        }
        
        /// <summary>
        /// This will sort the init parameters into a global dictionary
        /// of parameter => value
        /// </summary>
        /// <param name="args">The arguments as received from Main(args)</param>
        private void sortArgs(string[] args)
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
                // Same goes for -l and -c
                if (keyVal[0].Equals("-b") && val.Equals(""))
                    boardOverview = true;

                // We also have to check if this is a new card, 
                // for when adding descriptions. This will let us know,
                // if a --description parameter is incl. with a --new-card,
                // then it will be handled a little differently.
                // Same goes for newComment and newDescription
                if (keyVal[0].Equals("--new-card"))
                    newCard = true;
                if (keyVal[0].Equals("--description"))
                    newDescription = true;
                if (keyVal[0].Equals("--comment"))
                    newComment = true;
                
                // Will set global variables, so we know which board, list or card we are working with.
                if (keyVal[0].Equals("-b") && Validators.isInt(val))
                {
                    int.TryParse(val, out _board);
                    _board--;
                }

                if (keyVal[0].Equals("-l") && Validators.isInt(val))
                {
                    int.TryParse(val, out _list);
                    _list--;
                }

                if (keyVal[0].Equals("-c") && Validators.isInt(val))
                {
                    int.TryParse(val, out _card);
                    _card--;
                }

                Argument argument = new Argument {key = keyVal[0], value = val};
                argList.Add(argument);
            }
        }
    }
}