using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using clk.Controllers;
using clk.Models;
using clk.Resources;

namespace clk
{
    internal class Program
    {
        private static ArgumentController argController;

        private static OverviewController ovController;
        private static ListController liController;
        private static CardController caController;
        
        private static string boardId;
        private static string boardName;
        private static string listId;
        private static string listName;
        private static string cardId;
        private static string cardName;

        public static void Main(string[] args)
        {
            // Check that we have Json files
            Json.isFiles();
            
            // Manage the arguments in a separate controller
            argController = new ArgumentController(args);

            // Loop through all the parameters (args),
            // and perform the action associated with that parameter.
            foreach (IGrouping<string, string> keyVal in argController.getKeyVal())
            {
                if (keyVal.Key.Equals("-h"))
                    About.usage();
                 
                if (keyVal.Key.Equals("-b") && argController.boardOverview)
                    getBoards();
                
                if (keyVal.Key.Equals("-b") && argController.list < 0)
                    getLists();
                
                if (keyVal.Key.Equals("-l") && argController.card < 0 && !argController.newCard)
                    getCards();
                
                if (keyVal.Key.Equals("-c") && !argController.newComment && !argController.newDescription)
                    getCard();
                
                if (keyVal.Key.Equals("--new-board"))
                    createBoard(keyVal);
                
                if (keyVal.Key.Equals("--new-list"))
                    createList(keyVal);
                
                if (keyVal.Key.Equals("--new-card"))
                    createCard(keyVal);
                
                if (keyVal.Key.Equals("--description") && !argController.newCard)
                    createDescription(keyVal);
               
            }
        }
        
        /// <summary>
        /// If -b with no value is incl. this will run.
        /// It will invoke the OverviewController, get the boards,
        /// and print em out for the user.
        /// </summary>
        private static void getBoards()
        {
            iniOvController();
            
            int br = 0;
            Console.WriteLine("Available boards:");
            foreach (Board board in ovController.boards)
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
            if (argController.board < 0)
                return;

            // Initialize the required controllers and set some needed values
            iniOvController();
            iniLiController(boardId);
            
            int br = 0;

            // Print out the board information (lists)
            Console.WriteLine("Board: " + boardName);
            Console.WriteLine();
            Console.WriteLine("Available lists:");
            foreach (List list in liController.getLists())
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
            if (argController.board < 0 || argController.list < 0)
                return;

            // Initialize the required controllers and set some needed data
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            int br = 0;
            
            // Print out the list information (cards)
            Console.WriteLine("Board: " + boardName);
            Console.WriteLine("List : " + listName);
            Console.WriteLine();
            Console.WriteLine("Available cards:");
            
            foreach (Card card in caController.getCards())
            {
                br++;
                string cardCount = "  [" + br + "]: ";
                int cardCountLen = cardCount.Length + 1;
                Console.WriteLine(cardCount + card.name);
                if(!card.description.Equals(""))
                    Console.WriteLine(EyeCandy.indent(cardCountLen) + card.description);
            }

            Console.WriteLine();
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
            if (argController.board < 0 || argController.list < 0 || argController.card < 0)
                return;
            
            // Initialize the needed controllers, and get the required values
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            Card c = caController.getCards().Find(x => x.id == cardId);

            // Print out some godsend information about this selection!
            Console.WriteLine("Board: " + boardName);
            Console.WriteLine("List : " + listName);
            Console.WriteLine();
            Console.WriteLine("Card: " + c.name);
            Console.WriteLine(EyeCandy.indent(5) + c.description);
            
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
            iniOvController();
            foreach (var val in keyVal)
            {
                // Skip if the value is empty
                if (val.Equals(""))
                    continue;
                
                // Create the board
                ovController.createBoard(val);
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
            if (argController.board < 0)
                return;

            // Initialize the required controller and set some needed values
            iniOvController();
            iniLiController(boardId);
            
            // Loop through each of the --new-list and create lists accordingly
            foreach (var val in keyVal)
            {
                if (val.Equals(""))
                    continue;
                
                liController.createList(val);
                Console.WriteLine("Created list: " + val);
            }
            
            // Print out some sweet info for this selection
            Console.WriteLine("In board    : " + boardName);
            Console.WriteLine();
 
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
            if (argController.board < 0 || argController.list < 0)
                return;
            
            // Initialize the needed controllers and set some required data
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            // This will handle if you provide a --description along with a new card.
            string description = "";
            foreach (IGrouping<string, string> arg in argController.getKeyVal())
            {
                if (arg.Key.Equals("--description") 
                    && arg.Count() == 1
                    && !arg.FirstOrDefault().Equals(""))
                    description = arg.FirstOrDefault();
            }
            
            // Loop through all the --new-cards and create accordingly
            foreach (var val in keyVal)
            {   
                // Create the card
                caController.createCard(val, description);
                Console.WriteLine("Created card: " + val);

                // If a description was available, print that too
                if (!description.Equals(""))
                {
                    Console.WriteLine(EyeCandy.indent(2) + description);               
                }
            }
            
            // Print some good info for this selection
            Console.WriteLine("In list     : " + listName);
            Console.WriteLine("In board    : " + boardName);
            Console.WriteLine();

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
            if (argController.board < 0 || argController.list < 0 || argController.card < 0)
                return;

            // If description is empty, return
            if (keyVal.FirstOrDefault().Equals(""))
                return;
            
            
            // Initialize the needed controllers, and get the required values
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            caController.createDescription(keyVal.FirstOrDefault(), cardId);

            // Print out some info nice for this selection
            Console.WriteLine("Created description.");
            Console.WriteLine("In card : " + cardName);
            Console.WriteLine("In list : " + listName);
            Console.WriteLine("In board: " + boardName);
            Console.WriteLine();

            getCard();

        }

        /// <summary>
        /// Will initialize a OverviewController,
        /// and set the board-id and board-name for
        /// chosen board (from user parameter).
        /// </summary>
        private static void iniOvController()
        {
            ovController = new OverviewController();
            if (argController.board >= 0)
            {
                boardId = ObjectValues.getValueFromList(ovController.boards, argController.board, "id");
                boardName = ObjectValues.getValueFromList(ovController.boards, argController.board, "name");    
            }
        }

        /// <summary>
        /// Will initialize a ListController,
        /// as above, it will also set the name and the id
        /// chosen list. This requires the boardId where the list resides. 
        /// </summary>
        /// <param name="boardId">The board which holds the list</param>
        private static void iniLiController(string boardId)
        {
            liController = new ListController(boardId);
            if (argController.list >= 0)
            {
                listId = ObjectValues.getValueFromList(liController.getLists(), argController.list, "id");
                listName = ObjectValues.getValueFromList(liController.getLists(), argController.list, "name");   
            }
        }

        /// <summary>
        /// Will initialize a CardController,
        /// Again, name and id is set, and here we need the ID of the list,
        /// where the card(s) resides. 
        /// </summary>
        /// <param name="listId">The ID of the list, where the card(s) resides.</param>
        private static void iniCaController(string listId)
        {
            caController = new CardController(listId);
            if (argController.card >= 0)
            {
                cardId = ObjectValues.getValueFromList(caController.getCards(), argController.card, "id");
                cardName = ObjectValues.getValueFromList(caController.getCards(), argController.card, "name");    
            }
        }
    }
}