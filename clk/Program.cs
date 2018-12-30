using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
using clk.Controllers;
using clk.Models;
using clk.Resources;
using clk.Views;
using Newtonsoft.Json;

namespace clk
{
    internal class Program
    {
        private static string restUrl = "http://localhost:50066/Service1.svc/";
        //public static string restUrl = "http://easj-final.azurewebsites.net/Service1.svc/";
        
        public static Profile user = new Profile();

        public static ArgumentController argController;
        public static OverviewController ovController;
        public static ListController liController;
        public static CardController caController;

        public static Write write;

        // Latest arg input from the loop
        // on boards, lists, cards, checklists & points.
        public static int boardNum = -1;
        public static int listNum = -1;
        public static int cardNum = -1;
        public static int checkNum = -1;
        public static int pointNum = -1;
        
        // When the values are matches to an index,
        // The name and id of those elements will be defined.
        public static string boardId;
        public static string boardName;
        public static string listId;
        public static string listName;
        public static string cardId;
        public static string cardName;
        public static string checkId;
        public static string checkName;

        // These confirm, that the input value
        // indeed matches an index in one of the lists
        public static bool isBoard;
        public static bool isList;
        public static bool isCard;
        public static bool isCheck;

        public static void Main(string[] args)
        {
            
            Console.Clear();
            
            // Check that we have Json files
            Json.isFiles();
            
            // Controller for the arguments, and a "view" kinda object.
            write = new Write();
            argController = new ArgumentController(args);
            
            // Write top logo
            if (argController.isCard)
                Ascii.clkCard();
            else if (argController.isList)
                Ascii.clkList();
            else if (argController.isBoard 
                     && argController.argList.Find(x => x.key == "-b").value.Any())
                Ascii.clkBoard();
            else if (argController.isBoard)
                Ascii.clkBoards();
            else
                Ascii.clk();
            
            
            // Continuous running
            if (!args.Any())
            {
                Continuous c = new Continuous();
                c.loopApp();
                Environment.Exit(0);
            }

            // Send the merry argument on its way to its corresponding method
            foreach (Argument arg in argController.argList)
            {
                
                if (arg.key.Equals("-h"))
                    About.usage();
                
                if (arg.key.Equals("-b"))
                    getBoards(arg.value);
                
                if (arg.key.Equals("-l"))
                    getCards(arg.value);
                
                if (arg.key.Equals("-c")) 
                    getCard(arg.value);
                
                if (arg.key.Equals("-p"))
                    setChecklist(arg.value);
                
                if (arg.key.Equals("--point"))
                    clickPoint(arg.value);
                
                if (arg.key.Equals("--comment"))
                    createComment(arg.value);
                
                if (arg.key.Equals("--description"))
                    createDescription(arg.value);
                
                if (arg.key.Equals("--new-board"))
                    createBoard(arg.value);
                
                if (arg.key.Equals("--new-list"))
                    createList(arg.value);
                
                if (arg.key.Equals("--new-card"))
                    createCard(arg.value);
                
                if (arg.key.Equals("--new-check"))
                    createChecklist(arg.value);
                
                if (arg.key.Equals("--new-point"))
                    createChecklistPoint(arg.value);
                
                if (arg.key.Equals("--new-profile"))
                    createProfile();
                
                if (arg.key.Equals("--login"))
                    login();
                
                if (arg.key.Equals("--cloud-save"))
                    saveBoard();
                
                if (arg.key.Equals("--cloud-get"))
                    dbGetBoard(arg.value);
                
                if (arg.key.Equals("--cloud-boards"))
                    dbGetAllBoards();
                
                if (arg.key.Equals("--label"))
                    setLabel(arg.value);
                
                if (arg.key.Equals("--edit"))
                    edit(arg.value);
                
                if (arg.key.Equals("--del-board"))
                    deleteBoard(arg.value);
                
                if (arg.key.Equals("--del-list"))
                    deleteList(arg.value);
                
                if (arg.key.Equals("--del-card"))
                    deleteCard(arg.value);
                
                if (arg.key.Equals("--del-check"))
                    deleteChecklist(arg.value);
                
                if (arg.key.Equals("--del-point"))
                    deletePoint(arg.value);
                
                if (arg.key.Equals("--del-comment"))
                    deleteComment(arg.value);
                
                
                /*if (arg.key.Equals(""))
                    method(arg.value);*/
            }
            
            // Write the output for the selection
            // TODO: Casts exceptions on first creations of elements (no elements in lists).
            Console.WriteLine();
            if (argController.isCard && isCard)
                write.card(caController.cards.Find(x => x.id == cardId), caController);
            else if (argController.isList && isList)
                write.allCards(caController.getCards());
            else if (argController.isBoard && isBoard)
                write.allLists(liController.getLists());
            
            write.commentDestination();
        }

        /// <summary>
        /// If --p is incl. this will run.
        /// It will set the selected checklist,
        /// and is needed for adding new point to a specific checklist.
        /// </summary>
        /// <param name="args">List of Argument from the argument controller</param>
        private static void setChecklist(List<string> args)
        {
            if (!args.Any())
                return;

            if (!Validators.isInt(args.FirstOrDefault()))
                return;
            
            if (args.Count != 1)
                write.error("You can only work on 1 checklist at the time.");

            checkNum = int.Parse(args.FirstOrDefault());

            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            if (!isCheck)
                write.error("The selected checklist was not valid.");
            
            
        }


        #region Delete methods for Board, lists, checklists, points and comments
        
        /// <summary>
        /// If --del-board is incl. this is run.
        /// It will set a board to inactive (delete)
        /// </summary>
        /// <param name="args">User inputs</param>
        private static void deleteBoard(List<string> args)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            
            foreach (var val in args)
            {
                if (!Validators.isInt(val))
                    continue;
                
                if (!Validators.inList(ovController.getBoards(), int.Parse(val)))
                    write.error("The selected board was not valid.");

                Board b = ovController.getBoards()[int.Parse(val)];
                string name = b.name;

                if (!write.confirmDelete(name))
                    return;
                
                ovController.deleteBoard(b.id);
                Console.WriteLine("Deleted list: " + name);
            }
        }

        /// <summary>
        /// If --del-list is incl. this is run.
        /// It will set a list to inactive (delete)
        /// </summary>
        /// <param name="args">User input</param>
        private static void deleteList(List<string> args)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            
            if (!isBoard)
                write.error("The selected board was not valid.");
            
            foreach (var val in args)
            {
                if (!Validators.isInt(val))
                    continue;
                
                if (!Validators.inList(liController.getLists(), int.Parse(val)))
                    write.error("The selected list was not valid.");

                List l = liController.getLists()[int.Parse(val)];
                string name = l.name;
                
                if (!write.confirmDelete(name))
                    return;
                
                liController.deleteList(l.id);
                Console.WriteLine("Deleted list: " + name);
            }
        }

        /// <summary>
        /// If --del-card is incl. this will run.
        /// It will set a card to inactive (delete)
        /// </summary>
        /// <param name="args">User input</param>
        private static void deleteCard(List<string> args)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            
            if (!isList)
                write.error("The selected list was not valid.");
            
            foreach (var val in args)
            {
                if (!Validators.isInt(val))
                    continue;
                
                if (!Validators.inList(caController.getCards(), int.Parse(val)))
                    write.error("The selected card was not valid.");

                Card c = caController.getCards()[int.Parse(val)];
                string name = c.name;
                
                if (!write.confirmDelete(name))
                    return;
                
                caController.deleteCard(c.id);
                Console.WriteLine("Deleted card: " + name);
            }
        }

        /// <summary>
        /// If --del-check is incl. this wil run
        /// It will set a checklist to inactive (delete)
        /// </summary>
        /// <param name="args">Uset input</param>
        private static void deleteChecklist(List<string> args)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            if (!isCard)
                write.error("The selected card was not valid.");
            
            foreach (var val in args)
            {
                if (!Validators.isInt(val))
                    continue;
                
                if (!Validators.inList(caController.getChecklists(cardId), int.Parse(val)))
                    write.error("The selected checklist was not valid.");

                Checklist c = caController.getChecklists(cardId)[int.Parse(val)];
                string name = c.name;
                
                if (!write.confirmDelete(name))
                    return;
                
                caController.deleteChecklist(c.id);
                Console.WriteLine("Deleted checklist: " + name);
            }
        }

        /// <summary>
        /// If --del-point is incl. this will run.
        /// It will set a checklist to inactive (delete)
        /// </summary>
        /// <param name="args">User input</param>
        private static void deletePoint(List<string> args)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            if (!isCard)
                write.error("The selected card was not valid.");
            
            foreach (var val in args)
            {
                if (!Validators.isInt(val))
                    continue;
                
                if (!Validators.inList(caController.getChecklistPointsInCard(cardId), int.Parse(val)))
                    write.error("The selected checklist point was not valid.");

                ChecklistPoint p = caController.getChecklistPointsInCard(cardId)[int.Parse(val)];
                string name = p.name;
                
                if (!write.confirmDelete(name))
                    return;
                
                caController.deletePoint(p.id);
                Console.WriteLine("Deleted point: " + name);
            }
        }

        /// <summary>
        /// If --del-comment is incl. this will run.
        /// It will set a comment to inactive (delete)
        /// </summary>
        /// <param name="args">User input</param>
        private static void deleteComment(List<string> args)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            if (!isCard)
                write.error("The selected card was not valid.");
            
            foreach (var val in args)
            {
                if (!Validators.isInt(val))
                    continue;
                
                if (!Validators.inList(caController.getComments(cardId), int.Parse(val)))
                    write.error("The selected comment was not valid.");

                Comment c = caController.getComments(cardId)[int.Parse(val)];
                string date = c.created;
                
                if (!write.confirmDelete(date))
                    return;
                
                caController.deleteComment(c.id);
                Console.WriteLine("Deleted comment, created on: " + date);
            }
        }

        #endregion
        
        
        #region Update methods for Board, List, Card and Checklist (missing point atm)
       
        
        /// <summary>
        /// If --point is incl. this will run.
        /// It will check/uncheck a point in a checklist -
        /// Or at least invoke the controller, and let that handle it. 
        /// </summary>
        /// <param name="keyVal">KeyVal args from ToLookup (method in argument controller)</param>
        private static void clickPoint(List<string> args)
        {
            
            foreach (var val in args)
            {
                if (!Validators.isInt(val))
                    continue;
                
                // Initialize controllers and validate that the user-inputs are available in their respectful lists
                iniOvController();
                iniLiController(boardId);
                iniCaController(listId);
            
                if (!Validators.inList(caController.getChecklistPointsInCard(cardId), int.Parse(val)))
                    write.error("The selected point was not valid.");
            
            
                ChecklistPoint c = caController.getChecklistPointsInCard(cardId)[int.Parse(val)];
                caController.clickPoint(c.id);

                if (c.isCheck)
                    Console.WriteLine("Checked point: " + c.name);
                else
                    Console.WriteLine("Un-checked point: " + c.name);
            }
        }

        /// <summary>
        /// This will set a label for a card.
        /// Available colors are: red, green, yellow, blue and cyan.
        /// </summary>
        /// <param name="args"></param>
        private static void setLabel(List<string> args)
        {
            if (args.Count != 1)
                write.error("One 1 label can be assigned per card.");
            
            Label l = new Label();
            
            if (!l.isAvail(args.FirstOrDefault()))
                write.error("The selected label color was not available (red, green, yellow, blue & cyan).");

            l.label = args.FirstOrDefault();
            
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            if (!isCard)
                write.error("The selected card was not available.");

            caController.updateLabel(cardId, l);
        }

        /// <summary>
        /// If --edit is incl. this will run.
        /// This will figure out which element the user is trying to edit
        /// (Board, list, card, checklist)
        /// </summary>
        /// <param name="args">The argument --edit</param>
        private static void edit(List<string> args)
        {
            if (args.Count != 1)
                write.error("Edit only take 1 value.");

            if (args.FirstOrDefault().Equals(""))
                return;
            
            if (isCheck)
                editChecklist(args.FirstOrDefault());
            else if (isCard)
                editCard(args.FirstOrDefault());
            else if (isList)
                editList(args.FirstOrDefault());
            else if (isBoard)
                editBoard(args.FirstOrDefault());
            else
                write.error("No valid edit point was found.");
        }

        /// <summary>
        /// Edit a board (change name)
        /// It will use the boardId
        /// </summary>
        /// <param name="name">The new name</param>
        private static void editBoard(string name)
        {
            Board b = ovController.getBoards().Find(x => x.id == boardId);

            if (!write.confirmUpdate(b.name, name))
                return;

            boardName = name;
            ovController.updateBoard(name, boardId);
            Console.WriteLine("Updated board: " + name);
        }

        /// <summary>
        /// Edit a list (change name)
        /// It will use the listId
        /// </summary>
        /// <param name="name">The new name</param>
        private static void editList(string name)
        {
            List l = liController.lists.Find(x => x.id == listId);
            
            if (!write.confirmUpdate(l.name, name))
                return;
            
            listName = name;
            liController.updateList(name, listId);
            Console.WriteLine("Updated list: " + name);
        }

        /// <summary>
        /// Edit a card (change name)
        /// It will use the cardId
        /// </summary>
        /// <param name="name">The new name</param>
        private static void editCard(string name)
        {
            Card c = caController.cards.Find(x => x.id == cardId);
            
            if (!write.confirmUpdate(c.name, name))
                return;

            cardName = name;
            caController.updateCard(name, cardId);
            Console.WriteLine("Updated card: " + name);
        }

        /// <summary>
        /// Edit a checklist (change name)
        /// It will use the checkId
        /// </summary>
        /// <param name="name">The new name</param>
        private static void editChecklist(string name)
        {
            Checklist c = caController.checklists.Find(x => x.id == checkId);
            
            if (!write.confirmUpdate(c.name, name))
                return;

            checkName = name;
            caController.updateChecklist(name, checkId);
            Console.WriteLine("Updated checklist: " + name);
        }
        

        #endregion
        
        
        #region Init methods for each BoardController, ListController & CardController

        /// <summary>
        /// Will initialize a OverviewController,
        /// and set the board-id and board-name for
        /// chosen board (from user parameter).
        /// </summary>
        public static void iniOvController()
        {
            ovController = new OverviewController();
            isBoard = false;
            if (boardNum >= 0 && Validators.inList(ovController.getBoards(), boardNum))
            {
                isBoard = true;
                boardId = ObjectValues.getValueFromList(ovController.getBoards(), boardNum, "id");
                boardName = ObjectValues.getValueFromList(ovController.getBoards(), boardNum, "name");    
            }
        }

        /// <summary>
        /// Will initialize a ListController,
        /// as above, it will also set the name and the id
        /// chosen list. This requires the boardId where the list resides. 
        /// </summary>
        /// <param name="boardId">The board which holds the list</param>
        public static void iniLiController(string boardId)
        {
            liController = new ListController(boardId);
            isList = false;
            if (listNum >= 0 && Validators.inList(liController.getLists(), listNum))
            {
                isList = true;
                listId = ObjectValues.getValueFromList(liController.getLists(), listNum, "id");
                listName = ObjectValues.getValueFromList(liController.getLists(), listNum, "name");   
            }
        }

        /// <summary>
        /// Will initialize a CardController,
        /// Again, name and id is set, and here we need the ID of the list,
        /// where the card(s) resides. 
        /// </summary>
        /// <param name="listId">The ID of the list, where the card(s) resides.</param>
        public static void iniCaController(string listId)
        {
            caController = new CardController(listId);
            isCard = false;
            isCheck = false;
            
            if (cardNum >= 0 && Validators.inList(caController.getCards(), cardNum))
            {
                isCard = true;
                cardId = ObjectValues.getValueFromList(caController.getCards(), cardNum, "id");
                cardName = ObjectValues.getValueFromList(caController.getCards(), cardNum, "name");    
            }

            if (isCard 
                && checkNum >= 0
                && Validators.inList(caController.getChecklists(cardId), checkNum))
            {
                isCheck = true;
                checkId = ObjectValues.getValueFromList(caController.getChecklists(cardId), checkNum, "id");
                checkName = ObjectValues.getValueFromList(caController.getChecklists(cardId), checkNum, "name");
            }
        }
        
        #endregion

        
        #region Get methods for Boards, Lists, Cards and Card

        /// <summary>
        /// If -b with no value is incl. this will run.
        /// It will invoke the OverviewController, get the boards,
        /// and print em out for the user.
        /// </summary>
        private static void getBoards(List<string> args)
        {
            // Controller that holds boards
            iniOvController();
            
            if (args.Count == 0)
                write.allBoards(ovController.getBoards());
            else if (args.Count == 1)
                getLists(args);
            else // TODO: Why you no work?!?!?
                write.error("You are only able to work on 1 board at the time.");
        }

        /// <summary>
        /// If -b with a value is inc. this will run.
        /// It will display all the lists associated with the chosen board.
        /// The controller is making sure a relevant list of list, for the board,
        /// is returned.
        /// </summary>
        private static void getLists(List<string> args)
        {
            foreach (string arg in args)
            {
                // Make sure it is a numeric value
                if (!Validators.isInt(arg))
                    continue;
                
                // Set the boardNum to the selection
                boardNum = int.Parse(arg);
                
                // Initialize controllers 
                iniOvController();
                iniLiController(boardId);
        
                if (!isBoard)
                    write.error("The selected board was not valid.");
            }
        }

        /// <summary>
        /// If -b and -l, both with values, are incl. this is run.
        /// It will display all the cards for the associated list.
        /// The controller is making sure a relevant list of cards, for the list,
        /// is returned.
        /// </summary>
        private static void getCards(List<string> args)
        {
            
            foreach (string arg in args)
            {
                
                if (!Validators.isInt(arg))
                    continue;

                listNum = int.Parse(arg);
                
                // Initialize controllers and validate that the user-inputs are available in their respectful lists
                iniOvController();
                iniLiController(boardId);
                iniCaController(listId);

                if (!isList)
                    write.error("The selected list was not valid.");   
            }
        }

        /// <summary>
        /// If -c is incl. this will be run.
        /// It will print out information about a specific card.
        /// It will initialize the required controllers,
        /// and pick the selected Card from the list of cards (card controller).
        /// </summary>
        private static void getCard(List<string> args)
        {
            foreach (string arg in args)
            {
                if (!Validators.isInt(arg))
                    continue;

                cardNum = int.Parse(arg);
                
                // Initialize controllers and validate that the user-inputs are available in their respectful lists
                iniOvController();
                iniLiController(boardId);
                iniCaController(listId);
                
                if (!isCard)
                    write.error("The selected card was not valid.");
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
        private static void createBoard(List<string> args)
        {   
            
            iniOvController();
            foreach (var val in args)
            {
                if (val.Equals(""))
                    continue;

                // Create the board
                ovController.createBoard(val);
                Console.WriteLine("Created board: " + val);
                
                // Set a new boardNum val for the latest element
                boardNum = ovController.getBoards().Count - 1;
            }

            argController.isBoard = true;
            iniOvController();
        }

        /// <summary>
        /// If --new-list parameter is incl. this is run.
        /// It will make sure a board has been selected,
        /// find the ID of that board and initialize a
        /// ListController, which will handle the creation.
        /// </summary>
        /// <param name="keyVal">The KeyVal args from the ToLookup</param>
        private static void createList(List<string> args)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);

            if (!isBoard)
                write.error("The selected board was not valid.");
            
            // Loop through each of the --new-list and create lists accordingly
            foreach (var val in args)
            {
                if (val.Equals(""))
                    continue;
                
                liController.createList(val);
                Console.WriteLine("Created list : " + val);

                listNum = liController.getLists().Count - 1;
            }

            argController.isList = true;
            iniLiController(boardId);

        }

        /// <summary>
        /// If --new-card is incl. this is run.
        /// It will make sure that the board and list is selected,
        /// and let the ListController handle the creaton.
        /// </summary>
        /// <param name="keyVal">The KeyVal args from the ToLookup</param>
        private static void createCard(List<string> args)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);

            if (!isList)
                write.error("The selected list was not valid.");
            
            foreach (var val in args)
            {   
                // Create the card
                caController.createCard(val);
                Console.WriteLine("Created card: " + val);

                cardNum = caController.getCards().Count - 1;
            }

            argController.isCard = true;
            iniCaController(listId);
        }

        /// <summary>
        /// If --description is inkl. and you are not creating a new card, then this is run.
        /// It will make sure the board, list and card is selected,
        /// initialize the needed controllers and let the card controller create the description.
        /// </summary>
        /// <param name="keyVal">The KeyVal args from ToLookup</param>
        private static void createDescription(List<string> args)
        {
            if (!args.Any())
                return;
            
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            if (!isCard)
                write.error("The selected card was not valid.");
            
            caController.createDescription(args.FirstOrDefault(), cardId);
            Console.WriteLine("Created description: " + args.FirstOrDefault());
        }

        /// <summary>
        /// If --comment is incl. this will run.
        /// It will make sure board, list and card is set,
        /// initialize controllers for all three (board, list & card),
        /// and create each of the comments provided.
        /// </summary>
        /// <param name="keyVal">The arguments in ToLookup (argument controller method)</param>
        private static void createComment(List<string> args)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);

            if (!isCard)
                write.error("The selected card was not valid.");
            
            // Loop through and create each comment
            foreach (var val in args)
            {
                if (val.Equals(""))
                    continue;
                
                caController.createComment(val, cardId);
                Console.WriteLine("Created comment: " + val);
            }
        }

        /// <summary>
        /// If --new-check is incl. this will run.
        /// It will create a checklist for a card,
        /// if satisfied with the user inputs of course.
        /// </summary>
        /// <param name="keyVal">The ToLookup for the ArgumentController (args)</param>
        private static void createChecklist(List<string> args)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);

            if (!isCard)
                write.error("The selected card was not valid.");
            
            // Loop through each of the --check and --checkp, and create accordingly
            //string newChecklistId;
            foreach (var val in args)
            {
                if (val.Equals(""))
                    continue;
                
                caController.createChecklist(val, cardId);
                Console.WriteLine("Created checklist: " + val);

                checkNum = caController.getChecklists(cardId).Count - 1;
            }
        }

        /// <summary>
        /// If --new-point is incl. this will run.
        /// It will create a checklist point,
        /// if satisfied with the input parameters.
        /// </summary>
        /// <param name="keyVal"></param>
        private static void createChecklistPoint(List<string> args)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);

            if (!isCard)
                write.error("The selected card was not valid.");
            
            if (!Validators.inList(caController.getChecklists(cardId), checkNum))
                write.error("The selected checklist was not valid.");
            
            foreach (string val in args)
            {
                caController.createChecklistPoint(val, checkId);
                Console.WriteLine("Created checklist point: " + val);
            }
        }
        

        #endregion
        

        #region Cloud methods

        /// <summary>
        /// If --new-profile is incl. this will run.
        /// It will call the REST interface to create the profile.
        /// </summary>
        /// <param name="keyVal">ToLookup args from argController</param>
        private static void createProfile()
        {
            // Make sure we have a password
            if (user.password == null)
                write.error("Missing a password argument.");

            Console.WriteLine(user.password);
            
            RestClient rest = new RestClient(restUrl);

            try
            {
                rest.post(user, "profile/create");
            }
            catch (Exception e)
            {
                write.error(e.Message);
            }

            Console.WriteLine("Profile created.");
        }

        /// <summary>
        /// If --login is incl. this wil run.
        /// It will fetch the corresponding profile,
        /// from the database, to the provided values.
        /// If profile was not found, the user will not login.
        /// </summary>
        /// <param name="keyVal">From ToLookup method in argController</param>
        private static void login()
        {
            // Make sure we have a password
            if (user.password == null || user.email == null)
                write.error("Missing a login (password or email) argument.");

            RestClient client = new RestClient(restUrl);
            
            try
            {
                string c = client.post(user, "profile/login");
                Profile response = JsonConvert.DeserializeObject<Profile>(c);
                user.id = response.id;
                user.created = response.created;
                user.username = response.username;
            }
            catch (Exception e)
            {
                write.error(e.Message);
            }

            if (user.id == null)
                write.error("Username or password was incorrect.");
            
            Console.WriteLine("Successfully logged into: " + user.email);
        }

        /// <summary>
        /// Save a board to the cloud!
        /// </summary>
        private static void saveBoard()
        {
            iniOvController();

            // Gather the information needed for the http request
            BoardController bc = new BoardController(boardId);

            bc.userId = user.id;
            bc.password = user.password;

            RestClient rest = new RestClient(restUrl);

            try
            {
                rest.post(bc, "board/save");
            }
            catch (Exception e)
            {
                write.error(e.Message);
            }

            Console.WriteLine("Saved board to the cloud: " + bc.name);
        }

        private static void cloudBoards()
        {
            List<BoardController> response = dbGetAllBoards();
            int br = 1;
            Console.WriteLine("Available boards from the clouds:");
            foreach (BoardController bc in response)
            {
                Console.WriteLine("[" + br + "] " + bc.name);
                br++;
            }
        }

        /// <summary>
        /// Get all boards from the database (associated to the profile of course)
        /// </summary>
        /// <returns>The associated boards</returns>
        private static List<BoardController> dbGetAllBoards()
        {
            List<BoardController> response = new List<BoardController>();
            RestClient rest = new RestClient(restUrl);
            try
            {
                string c = rest.post(user, "board/getall");
                response = JsonConvert.DeserializeObject<List<BoardController>>(c);
            }
            catch (Exception e)
            {
                write.error(e.Message);
            }

            return response;
        }

        private static void dbGetBoard(List<string> args)
        {
            //IList<BoardController> boardsAvail = dbGetAllBoards();

            // TODO: Loop through all selections

            // TODO: tryparse
            /*int selection = int.Parse(argController.getKeyVal()["--cloud-get"].First());
            selection--;*/

            //if (boardsAvail[selection] == null)
            //    return;

            RestClient rest = new RestClient(restUrl);
            string c = rest.post(user, "board/get/54f22eff-94d5-4d31-83ec-b1fb815e3c4a");
            Console.WriteLine(c);
        }

        #endregion
           
    }
}