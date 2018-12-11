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
        private static bool newCard;      
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
                 
                if (keyVal.Key.Equals("-b") && list < 1)
                    getLists();
                
                if (keyVal.Key.Equals("-l") && card < 1)
                    getCards();
                
                if (keyVal.Key.Equals("-c"))
                    getCard();
                
                if (keyVal.Key.Equals("-b") && boardOverview)
                    getBoards();
                
                if (keyVal.Key.Equals("--new-board"))
                    createBoard(keyVal);
                
                if (keyVal.Key.Equals("--new-list"))
                    createList(keyVal);
                
                if (keyVal.Key.Equals("--new-card"))
                    createCard(keyVal);
                
                if (keyVal.Key.Equals("--description") && !newCard)
                    createDescription(keyVal);
               
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
                // Same goes for -l and -c
                if (keyVal[0].Equals("-b") && val.Equals(""))
                    boardOverview = true;

                // We also have to check if this is a new card, 
                // for when adding descriptions. This will let us know,
                // if a --description parameter is incl. with a --new-card,
                // then it will be handled a little differently.
                if (keyVal[0].Equals("--new-card"))
                    newCard = true;
                
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
                Console.WriteLine("  ["+ br +"]: " + board.name);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// If -b with a value is inc. this will run.
        /// It will display all the lists associated with the chosen board.
        /// The controller is making sure a relevant list of list, for the board,
        /// is returned.
        /// </summary>
        private static void getLists()
        {
            // Make sure board is selected
            if (board < 1)
                return;

            // Correct values for list indexing
            board--;
            
            // Initialize the required controllers and set some needed values
            OverviewController ovController = new OverviewController();
            string boardId = ObjectValues.getValueFromList(ovController.boards, board, "id");
            string boardName = ObjectValues.getValueFromList(ovController.boards, board, "name");
            
            ListController controller = new ListController(boardId);
            int br = 0;

            // Print out the board information (lists)
            Console.WriteLine("Board: " + boardName);
            Console.WriteLine();
            Console.WriteLine("Available lists:");
            foreach (List list in controller.getLists())
            {
                br++;
                Console.WriteLine("  ["+ br +"]: " + list.name);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// If -b and -l, both with values, are incl. this is run.
        /// It will display all the cards for the associated list.
        /// The controller is making sure a relevant list of cards, for the list,
        /// is returned.
        /// </summary>
        private static void getCards()
        {
            // Make sure board and list is selected
            if (board < 1 || list < 1)
                return;

            // Correct values for list indexing
            board--;
            list--;

            Console.WriteLine(board);
            Console.WriteLine(list);
            
            // Initialize the required controllers and set some needed data
            OverviewController ovController = new OverviewController();
            string boardId = ObjectValues.getValueFromList(ovController.boards, board, "id");
            string boardName = ObjectValues.getValueFromList(ovController.boards, board, "name");
            
            ListController liController = new ListController(boardId);
            string listId = ObjectValues.getValueFromList(liController.getLists(), list, "id");
            string listName = ObjectValues.getValueFromList(liController.getLists(), list, "name");
            
            CardController controller = new CardController(listId);
            int br = 0;
            
            // Print out the list information (cards)
            Console.WriteLine("Board: " + boardName);
            Console.WriteLine("List : " + listName);
            Console.WriteLine();
            Console.WriteLine("Available cards:");
            
            foreach (Card card in controller.getCards())
            {
                br++;
                string cardCount = "  [" + br + "]: ";
                int cardCountLen = cardCount.Length;
                Console.WriteLine(cardCount + card.name);
                Console.WriteLine(card.description.PadLeft(cardCountLen, ' '));
                
                Console.WriteLine();
            }
        }

        /// <summary>
        /// If -c is incl. this will be run.
        /// It will print out information about a specific card.
        /// It will initialize the required controllers,
        /// and pick the selected Card from the list of cards (card controller).
        /// </summary>
        private static void getCard()
        {
            // Make sure board, list and card is selected
            if (board < 1 || list < 1 || card < 1)
                return;

            // Correct values for list indexing
            board--;
            list--;
            card--;
            
            // Initialize the needed controllers, and get the required values
            OverviewController ovController = new OverviewController();
            string boardId = ObjectValues.getValueFromList(ovController.boards, board, "id");
            string boardName = ObjectValues.getValueFromList(ovController.boards, board, "name");
            
            ListController liController = new ListController(boardId);
            string listId = ObjectValues.getValueFromList(liController.getLists(), list, "id");
            string listName = ObjectValues.getValueFromList(liController.getLists(), list, "name");
            
            CardController controller = new CardController(listId);
            string cardId = ObjectValues.getValueFromList(controller.getCards(), card, "id");
            string cardName = ObjectValues.getValueFromList(controller.getCards(), card, "name");

            Card c = controller.getCards().Find(x => x.id == cardId);

            // Print out some godsend information about this selection!
            Console.WriteLine("Card: " + c.name);
            Console.WriteLine(c.description.PadLeft(8, ' '));
            
            // TODO: Here'll be checklists too!
        }

        /// <summary>
        /// If --new-board parameter is incl. this is run.
        /// It will invoke the OverviewController (boards).
        /// The controller will handle the heavy work. 
        /// </summary>
        /// <param name="keyVal">The KeyVal args from the ToLookup</param>
        private static void createBoard(IGrouping<string, string> keyVal)
        {   
            // Initialize the controller and loop through all the --new-board to create accordingly
            OverviewController controller = new OverviewController();
            foreach (var val in keyVal)
            {
                // Skip if the value is empty
                if (val.Equals(""))
                    continue;
                
                // Create the board
                controller.createBoard(val);
                Console.WriteLine("Created board: " + val);
            }

            Console.WriteLine();
            getBoards();
        }

        /// <summary>
        /// If --new-list parameter is incl. this is run.
        /// It will make sure a board has been selected,
        /// find the ID of that board and initialize a
        /// ListController, which will handle the creation.
        /// </summary>
        /// <param name="keyVal">The KeyVal args from the ToLookup</param>
        private static void createList(IGrouping<string, string> keyVal)
        {
            // Check that board is set
            if (board < 1)
                return;

            // Correct for list indexing
            board--;
            
            // Initialize the required controller and set some needed values
            OverviewController ovController = new OverviewController();
            string boardId = ObjectValues.getValueFromList(ovController.boards, board, "id");
            string boardName = ObjectValues.getValueFromList(ovController.boards, board, "name");
            
            ListController controller = new ListController(boardId);
            
            // Loop through each of the --new-list and create lists accordingly
            foreach (var val in keyVal)
            {
                if (val.Equals(""))
                    continue;
                
                controller.createList(val);
                Console.WriteLine("Created list: " + val);
            }
            
            // Print out some sweet info for this selection
            Console.WriteLine("In board    : " + boardName);
            Console.WriteLine();

            board++; //will be subtracted again in the getList() method 
            getLists();
        }

        /// <summary>
        /// If --new-card is incl. this is run.
        /// It will make sure that the board and list is selected,
        /// and let the ListController handle the creaton.
        /// </summary>
        /// <param name="keyVal">The KeyVal args from the ToLookup</param>
        private static void createCard(IGrouping<string, string> keyVal)
        {
            // Check that board and list is set
            if (board < 1 || list < 1)
                return;

            // Correct values for list indexing
            board--;
            list--;
            
            // Initialize the needed controllers and set some required data
            OverviewController ovController = new OverviewController();
            string boardId = ObjectValues.getValueFromList(ovController.boards, board, "id");
            string boardName = ObjectValues.getValueFromList(ovController.boards, board, "name");
            
            ListController liController = new ListController(boardId);
            string listId = ObjectValues.getValueFromList(liController.getLists(), list, "id");
            string listName = ObjectValues.getValueFromList(liController.getLists(), list, "name");
            
            CardController controller = new CardController(listId);
            
            // Loop through all the --new-cards and create accordingly
            foreach (var val in keyVal)
            {
                
                // This will handle if you provide a --description along with a new card.
                string description = "";
                foreach (IGrouping<string, string> arg in argList)
                {
                    if (arg.Key.Equals("--description") 
                            && !arg.FirstOrDefault().Equals("")
                            && arg.Count() == 1)
                        description = arg.FirstOrDefault();
                }
                
                // Create the card
                controller.createCard(val);
                Console.WriteLine("Created card: " + val);

                // If a description was available, print that too
                if (!description.Equals(""))
                {
                    Console.WriteLine("With description: " + description);
                    Console.WriteLine();                    
                }
            }
            
            // Print some good info for this selection
            Console.WriteLine("In list     : " + listName);
            Console.WriteLine("In board    : " + boardName);
            Console.WriteLine();

            board++;
            list++; // These two will be subtracted again in getCards()
            getCards();
        }

        /// <summary>
        /// If --description is inkl. and you are not creating a new card, then this is run.
        /// It will make sure the board, list and card is selected,
        /// initialize the needed controllers and let the card controller create the description.
        /// </summary>
        /// <param name="keyVal">The KeyVal args from ToLookup</param>
        private static void createDescription(IGrouping<string, string> keyVal)
        {
            // Make sure board, list and card is selected
            if (board < 1 || list < 1 || card < 1)
                return;

            // If description is empty, return
            if (keyVal.FirstOrDefault().Equals(""))
                return;
            
            // Correct values for list indexing
            board--;
            list--;
            card--;
            
            // Initialize the needed controllers, and get the required values
            OverviewController ovController = new OverviewController();
            string boardId = ObjectValues.getValueFromList(ovController.boards, board, "id");
            string boardName = ObjectValues.getValueFromList(ovController.boards, board, "name");
            
            ListController liController = new ListController(boardId);
            string listId = ObjectValues.getValueFromList(liController.getLists(), list, "id");
            string listName = ObjectValues.getValueFromList(liController.getLists(), list, "name");
            
            CardController controller = new CardController(listId);
            string cardId = ObjectValues.getValueFromList(controller.getCards(), card, "id");
            string cardName = ObjectValues.getValueFromList(controller.getCards(), card, "name");
            
            controller.createDescription(keyVal.FirstOrDefault(), cardId);

            // Print out some info nice for this selection
            Console.WriteLine("Created description.");
            Console.WriteLine("In card : " + cardName);
            Console.WriteLine("In list : " + listName);
            Console.WriteLine("In board: " + boardName);
            Console.WriteLine();

            board++;
            list++;
            card++; // These three will be subtracted again in getCard()
            getCard();

        }
    }
}