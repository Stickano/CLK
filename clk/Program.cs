using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public static Profile user = new Profile();

        private static ArgumentController argController;
        private static OverviewController ovController;
        private static ListController liController;
        private static CardController caController;

        private static Write write;

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
        private static bool isBoard;
        private static bool isList;
        private static bool isCard;
        private static bool isCheck;

        public static void Main(string[] args)
        {
            
            if (!args.Any())
                About.usage(); //TODO: continious running
            
            // Check that we have Json files
            Json.isFiles();
            
            write = new Write();
            argController = new ArgumentController(args);
            
            
            // Write top logo
            if (argController.isCard)
                Ascii.clkCard();
            else if (argController.isList)
                Ascii.clkList();
            else if (argController.isBoard)
                Ascii.clkBoard();

            foreach (Argument arg in argController.argList)
            {
                
                if (arg.key.Equals("-h"))
                    About.usage();
                
                if (arg.key.Equals("-b"))
                    getBoards(arg.value);
                
                if (arg.key.Equals("-l"))
                    getCards(arg.value);
                
                if (arg.key.Equals("-c")) //TODO: 
                    getCard(arg.value);
                
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
                
                /*if (arg.key.Equals(""))
                    method(arg.value);*/
            }
            
            write.commentDestination();
        }
        
        
        #region Init methods for each BoardController, ListController & CardController

        /// <summary>
        /// Will initialize a OverviewController,
        /// and set the board-id and board-name for
        /// chosen board (from user parameter).
        /// </summary>
        private static void iniOvController()
        {
            ovController = new OverviewController();
            isBoard = false;
            if (boardNum >= 0 && Validators.inList(ovController.boards, boardNum))
            {
                isBoard = true;
                boardId = ObjectValues.getValueFromList(ovController.boards, boardNum, "id");
                boardName = ObjectValues.getValueFromList(ovController.boards, boardNum, "name");    
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
        private static void iniCaController(string listId)
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
                write.allBoards(ovController.boards);
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
                
                if (argController.isList)
                    break;
            
                write.allLists(liController.getLists());
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
                
                if (argController.isCard)
                    break;
            
                write.allCards(caController.getCards());   
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
            
                Card c = caController.getCards().Find(x => x.id == cardId);
                write.card(c, caController);   
            }
        }
        

        #endregion

        /// <summary>
        /// If --point is incl. this will run.
        /// It will check/uncheck a point in a checklist -
        /// Or at least invoke the controller, and let that handle it. 
        /// </summary>
        /// <param name="keyVal">KeyVal args from ToLookup (method in argument controller)</param>
        public static void clickPoint(List<string> args)
        {
            
            foreach (var val in args)
            {
                if (!Validators.isInt(val))
                    continue;
                
                // Initialize controllers and validate that the user-inputs are available in their respectful lists
                iniOvController();
                iniLiController(boardId);
                iniCaController(listId);
            
                if (!isCheck)
                    write.error("The selected checklist was not valid.");
            
                if (!Validators.inList(caController.getChecklistPoints(checkId), int.Parse(val)))
                    write.error("The selected point was not valid.");
            
            
                ChecklistPoint c = caController.getChecklistPoints(checkId)[int.Parse(val)];
                caController.clickPoint(c.id);

                if (c.isCheck)
                    Console.WriteLine("Checked point: " + c.name);
                else
                    Console.WriteLine("Un-checked point: " + c.name);
            }
        }
        
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
                boardNum = ovController.boards.Count - 1;
            }
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
        }

        /// <summary>
        /// If --description is inkl. and you are not creating a new card, then this is run.
        /// It will make sure the board, list and card is selected,
        /// initialize the needed controllers and let the card controller create the description.
        /// </summary>
        /// <param name="keyVal">The KeyVal args from ToLookup</param>
        private static void createDescription(List<string> args)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            if (!isCard)
                write.error("The selected card was not valid.");
            
            caController.createDescription(args.FirstOrDefault(), cardId);
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

                checkNum = caController.checklists.Count - 1;
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

            RestClient rest = new RestClient(restUrl);
            rest.post(user, "profile/create");
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
            if (user.password == null)
                write.error("Missing a password argument.");

            RestClient client = new RestClient(restUrl);
            string c = client.post(user, "profile/login");
            
            Profile response = JsonConvert.DeserializeObject<Profile>(c);
            if (response.id == null)
            {
                Ascii.ahahah();
                Environment.Exit(0);
            }

            user.id = response.id;
            user.created = response.created;
            user.username = response.username;
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
            rest.post(bc, "board/save");
        }

        
        private static IList<BoardController> dbGetAllBoards()
        {
            RestClient rest = new RestClient(restUrl);
            string c = rest.post(user, "board/getall");
            Console.WriteLine(c);
            IList<BoardController> response = JsonConvert.DeserializeObject<IList<BoardController>>(c);

            foreach (BoardController bc in response)
            {
                Console.WriteLine(bc.name);
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