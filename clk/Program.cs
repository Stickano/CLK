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

        private static bool isBoard;
        private static bool isList;
        private static bool isCard;

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
                 
                if (keyVal.Key.Equals("-b") 
                    && argController.boardOverview)
                    getBoards();
                
                if (keyVal.Key.Equals("-b") 
                    && argController.list < 0)
                    getLists();
                
                if (keyVal.Key.Equals("-l") 
                    && argController.card < 0 
                    && !argController.newCard)
                    getCards();
                
                if (keyVal.Key.Equals("-c") 
                    && !argController.newComment 
                    && !argController.newDescription
                    && !argController.newCheck
                    && !argController.newPoint)
                    getCard();
                
                if (keyVal.Key.Equals("--new-board"))
                    createBoard(keyVal);
                
                if (keyVal.Key.Equals("--new-list"))
                    createList(keyVal);
                
                if (keyVal.Key.Equals("--new-card"))
                    createCard(keyVal);
                
                if (keyVal.Key.Equals("--new-check"))
                    createChecklist(keyVal);
                
                if (keyVal.Key.Equals("--new-point") 
                    && !argController.newCheck)
                    createChecklistPoint(keyVal);
                
                if (keyVal.Key.Equals("--description") 
                    && !argController.newCard)
                    createDescription(keyVal);
                
                if (keyVal.Key.Equals("--comment") 
                    && !argController.newCard)
                    createComment(keyVal);
            }
        }

        #region Get methods for Boards, Lists, Cards and Card

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
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            
            if (!isBoard)
                return;
            
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
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);

            if (!isList)
                return;
            
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
                int cardCountLen = cardCount.Length;
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
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            if (!isCard)
                return;
            
            Card c = caController.getCards().Find(x => x.id == cardId);

            // Print out some godsend information about this selection!
            Console.WriteLine("Board: " + boardName);
            Console.WriteLine("List : " + listName);
            Console.WriteLine();
            Console.WriteLine("Card: " + c.name);
            Console.WriteLine(EyeCandy.indent(6) + c.description);

            int chBr = 0;
            Console.WriteLine();
            Console.WriteLine("Checklist(s):");
            foreach (Checklist checklist in caController.getChecklists(cardId))
            {
                chBr++;
                string outCount = "  [" + chBr + "]: ";
                int outCountLen = outCount.Length;
                Console.WriteLine(outCount + checklist.name);

                int pBr = 0;
                foreach (ChecklistPoint point in caController.getChecklistPoints(checklist.id))
                {
                    pBr++;
                    string outCountP = "[" + pBr + "]: ";
                    Console.WriteLine(EyeCandy.indent(outCountLen) + outCountP + point.name);
                }

                Console.WriteLine();
            }

            int br = 0;
            Console.WriteLine();
            Console.WriteLine("Comment(s):");
            foreach (Comment comment in caController.getComments(cardId))
            {
                br++;
                string outCount = "  [" + br + "]: ";
                int outCountLen = outCount.Length;
                Console.WriteLine(outCount + "(" + comment.created + ")");
                Console.WriteLine(EyeCandy.indent(outCountLen) + comment.comment);
                Console.WriteLine();
            }
        }
        

        #endregion

        #region Create methods for Boards, Lists, Cards, Comments, Descriptions, Checklists & points

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
            
            // If --new-list is included as parameter, create those for the board(s) too.
            List<string> lists = new List<string>();
            foreach (var arg in argController.getKeyVal()["--new-list"])
            {
                if (arg.Equals(""))
                    continue;

                lists.Add(arg);
            }
            
            string newBoardId;
            foreach (var val in keyVal)
            {
                // Skip if the value is empty
                if (val.Equals(""))
                    continue;
                
                // Create the board
                newBoardId = ovController.createBoard(val);
                Console.WriteLine("Created board: " + val);

                // Handle new lists
                if (lists.Count > 0)
                {
                    ListController li =  new ListController(newBoardId);
                    foreach (string list in lists)
                    {
                        li.createList(list);
                        Console.WriteLine("Added list: " + list);
                    }
                }
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
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);

            if (!isBoard)
                return;
            
            // TODO: Create all cards too
            
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
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);

            if (!isList)
                return;
            
            // This will handle if you provide a --description along with a new card.
            string description = argController.getKeyVal()["--description"].First();

            // If --comment is included as parameter, create those for the card too.
            List<string> comments = new List<string>();
            foreach (var arg in argController.getKeyVal()["--comment"])
            {
                if (arg.Equals(""))
                    continue;

                comments.Add(arg);
            }
            
            // Loop through all the --new-cards and create accordingly
            string newCardId;
            foreach (var val in keyVal)
            {   
                // Create the card
                newCardId = caController.createCard(val, description);
                Console.WriteLine("Created card: " + val);

                // If a description was available, print that too
                if (!description.Equals(""))
                    Console.WriteLine(EyeCandy.indent(2) + description);
                
                // And the comments
                if (comments.Count > 0)
                {
                    Console.WriteLine();
                    foreach (string comment in comments)
                    {
                        caController.createComment(comment, newCardId);
                        Console.WriteLine("Added comment: " + comment);
                    }
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
            // If description is empty, return
            if (keyVal.FirstOrDefault().Equals("")) //TODO: This is kinda sketchy.. Fix so you can loop through perhaps.
                return;
            
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            if (!isCard)
                return;
            
            caController.createDescription(keyVal.FirstOrDefault(), cardId);

            // Print out some info nice for this selection
            Console.WriteLine("Created description: " + keyVal.FirstOrDefault());
            Console.WriteLine("In card : " + cardName);
            Console.WriteLine("In list : " + listName);
            Console.WriteLine("In board: " + boardName);
            Console.WriteLine();

            getCard();
        }

        /// <summary>
        /// If --comment is incl. this will run.
        /// It will make sure board, list and card is set,
        /// initialize controllers for all three (board, list & card),
        /// and create each of the comments provided.
        /// </summary>
        /// <param name="keyVal">The arguments in ToLookup (argument controller method)</param>
        private static void createComment(IGrouping<string, string> keyVal)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);

            if (!isCard)
                return;
            
            // Loop through and create each comment
            foreach (var val in keyVal)
            {
                if (val.Equals(""))
                    continue;
                
                caController.createComment(val, cardId);
                Console.WriteLine("Created comment: " + val);
            }
            
            // Print some good info for this selection
            Console.WriteLine("In card     : " + cardName);
            Console.WriteLine("In list     : " + listName);
            Console.WriteLine("In board    : " + boardName);
            Console.WriteLine();

            getCard();
        }

        /// <summary>
        /// If --new-check is incl. this will run.
        /// It will create a checklist for a card,
        /// if satisfied with the user inputs of course.
        /// </summary>
        /// <param name="keyVal">The ToLookup for the ArgumentController (args)</param>
        private static void createChecklist(IGrouping<string, string> keyVal)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);

            if (!isCard)
                return;
            
            // If --checkp (points) is included as parameter, create those for the checklist too.
            List<string> points = new List<string>();
            foreach (var arg in argController.getKeyVal()["--new-point"])
            {
                if (arg.Equals(""))
                    continue;

                points.Add(arg);
            }

            // Loop through each of the --check and --checkp, and create accordingly
            string newChecklistId;
            foreach (var val in keyVal)
            {
                if (val.Equals(""))
                    continue;
                
                newChecklistId = caController.createChecklist(val, cardId);
                Console.WriteLine("Created checklist: " + val);

                foreach (string point in points)
                {
                    caController.createChecklistPoint(point, newChecklistId);
                    Console.WriteLine("Added point      : " + point);
                }
            }
            
            // Print some good info for this selection
            Console.WriteLine("In card          : " + cardName);
            Console.WriteLine("In list          : " + listName);
            Console.WriteLine("In board         : " + boardName);
            Console.WriteLine();

            getCard();
        }

        /// <summary>
        /// If --new-point is incl. this will run.
        /// It will create a checklist point,
        /// if satisfied with the input parameters.
        /// </summary>
        /// <param name="keyVal"></param>
        private static void createChecklistPoint(IGrouping<string, string> keyVal)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);

            if (!isCard)
                return;

            if (!Validators.inList(caController.getChecklists(cardId), argController.check))
                return;
            
            string checklistId = ObjectValues.getValueFromList(caController.getChecklists(cardId), argController.check, "id");
            string checklistName = caController.getChecklists(cardId)
                .Find(x => x.id == checklistId)
                .name;

            foreach (string val in keyVal)
            {
                caController.createChecklistPoint(val, checklistId);
                Console.WriteLine("Created checklist point: " + val);
            }

            // Print some good info for this selection
            Console.WriteLine("In checklist           : " + checklistName);
            Console.WriteLine("In card                : " + cardName);
            Console.WriteLine("In list                : " + listName);
            Console.WriteLine("In board               : " + boardName);
            Console.WriteLine();

            getCard();
        }
        

        #endregion
        
        #region Init methods for each BoardController, ListController & CardController

        /// <summary>
        /// Will initialize a OverviewController,
        /// and set the board-id and board-name for
        /// chosen board (from user parameter).
        /// </summary>
        private static void iniOvController()
        {
            ovController = new OverviewController();
            if (argController.board >= 0 && Validators.inList(ovController.boards, argController.board))
            {
                isBoard = true;
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
            if (argController.list >= 0 && Validators.inList(liController.getLists(), argController.list))
            {
                isList = true;
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
            if (argController.card >= 0 && Validators.inList(caController.getCards(), argController.card))
            {
                isCard = true;
                cardId = ObjectValues.getValueFromList(caController.getCards(), argController.card, "id");
                cardName = ObjectValues.getValueFromList(caController.getCards(), argController.card, "name");    
            }
        }
        
        #endregion
        
    }
}