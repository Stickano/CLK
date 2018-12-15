using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using clk.Controllers;
using clk.Models;
using clk.Resources;
using Newtonsoft.Json;

namespace clk
{
    internal class Program
    {
        private static string restUrl = "http://localhost:50066/Service1.svc/";

        private static Profile user;

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
        private static string checkId;
        private static string checkName;

        private static bool isBoard;
        private static bool isList;
        private static bool isCard;
        private static bool isCheck;

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
                //TODO: FIX THIS MESS
                if (keyVal.Key.Equals("-h"))
                    About.usage();
                 
                if (keyVal.Key.Equals("-b") 
                    && argController.boardOverview)
                    getBoards();
                
                if (keyVal.Key.Equals("-b") 
                    && argController.list < 0 
                    && !argController.newList)
                    getLists();
                
                if (keyVal.Key.Equals("-l") 
                    && argController.card < 0 
                    && !argController.newCard)
                    getCards();
                
                if (keyVal.Key.Equals("-c") 
                    && !argController.newComment 
                    && !argController.newDescription
                    && !argController.newCheck
                    && !argController.newPoint
                    && argController.point < 0)
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
                
                if (keyVal.Key.Equals("--point"))
                    clickPoint(keyVal);

                if (keyVal.Key.Equals("--new-profile"))
                    createProfile(keyVal);

                if (keyVal.Key.Equals("--login"))
                    login(keyVal);

                if (keyVal.Key.Equals("--save-board"))
                    saveBoard();
            }
        }

        #region Get methods for Boards, Lists, Cards and Card

        /// <summary>
        /// If -b with no value is incl. this will run.
        /// It will invoke the OverviewController, get the boards,
        /// and print em out for the user.
        /// </summary>
        private static void getBoards(bool logo=true)
        {
            // Controller that holds boards
            iniOvController();
            
            // Print logo
            if (logo && argController.easter)
                Ascii.clkEaster();
            else if (logo)
                Ascii.clk();

            // Loop through all boards
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
        private static void getLists(bool logo=true)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            
            if (!isBoard)
                return;
            
            if (logo)
                Ascii.clkBoard();

            // Print out the board information (lists)
            commentDestination(boardName);
            Console.WriteLine("Available lists:");
            
            int br = 0;
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
        private static void getCards(bool logo=true)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);

            if (!isList)
                return;

            if (logo)
                Ascii.clkList();

            // Print out the list information (cards)
            commentDestination(boardName, listName);
            Console.WriteLine("Available cards:");
            int br = 0;
            foreach (Card card in caController.getCards())
            {
                br++;
                string cardCount = "  [" + br + "]: ";
                int cardCountLen = cardCount.Length;
                Console.WriteLine(cardCount + card.name);
                if(card.description != null && !card.description.Equals(""))
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
        private static void getCard(bool logo=true)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            if (!isCard)
                return;
            
            if (logo)
                Ascii.clkCard();
            
            Card c = caController.getCards().Find(x => x.id == cardId);

            // Print out some godsend information about this selection!
            commentDestination(boardName, listName, c.name);
            Console.WriteLine(EyeCandy.indent(6) + c.description);
            Console.WriteLine();

            // Checklist section
            int chBr = 0;
            Console.WriteLine("Checklist(s):");
            foreach (Checklist checklist in caController.getChecklists(cardId))
            {
                chBr++;
                string outCount = "  [" + chBr + "]: ";
                int outCountLen = outCount.Length;
                Console.WriteLine(outCount + checklist.name);

                // Checklist points
                int pBr = 0;
                foreach (ChecklistPoint point in caController.getChecklistPoints(checklist.id))
                {
                    pBr++;
                    string outCountP = "[" + pBr + "]: ";

                    // Colors, colors everywhere!
                    if (point.isCheck)
                        EyeCandy.color("green");
                    else
                        EyeCandy.color("yellow");
                    
                    Console.WriteLine(EyeCandy.indent(outCountLen) + outCountP + point.name);
                    EyeCandy.reset();
                }

                Console.WriteLine();
            }

            // Comment section
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

            Console.WriteLine();
        }
        

        #endregion

        /// <summary>
        /// If --point is incl. this will run.
        /// It will check/uncheck a point in a checklist -
        /// Or at least invoke the controller, and let that handle it. 
        /// </summary>
        /// <param name="keyVal">KeyVal args from ToLookup (method in argument controller)</param>
        public static void clickPoint(IGrouping<string, string> keyVal)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            if (!isCard)
                return;
            
            if (!Validators.inList(caController.getChecklistPoints(checkId), argController.point))
                return;
            
            Ascii.clkCard();

            // Loop through all the points and check/uncheck em'
            foreach (var val in keyVal)
            {
                if (!Validators.isInt(val))
                    continue;
                
                int parseArg;
                int.TryParse(val, out parseArg);
                parseArg--;
                
                ChecklistPoint c = caController.getChecklistPoints(checkId)[parseArg];
                caController.clickPoint(c.id);

                if (c.isCheck)
                    Console.WriteLine("Checked point: " + c.name);
                else
                    Console.WriteLine("Un-checked point: " + c.name);
            }
            
            commentAction(boardName, listName, cardName, checkName);
            getCard(false);
        }
        
        

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
            
            Ascii.clk();
            
            // If --new-list is included as parameter, create those for the board(s) too.
            List<string> lists = new List<string>();
            foreach (var arg in argController.getKeyVal()["--new-list"])
            {
                if (arg.Equals(""))
                    continue;

                lists.Add(arg);
            }
            
            // Loop through all the --new-boards and create accordingly
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
            getBoards(false);
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
            
            Ascii.clkBoard();
            
            // TODO: Create all cards too
            
            // Loop through each of the --new-list and create lists accordingly
            foreach (var val in keyVal)
            {
                if (val.Equals(""))
                    continue;
                
                liController.createList(val);
                Console.WriteLine("Created list : " + val);
            }
            
            // Print out some sweet info for this selection
            commentAction(boardName);
            getLists(false);
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
            
            Ascii.clkList();
            
            // This will handle if you provide a --description along with a new card.
            string description = "";
            if (argController.getKeyVal()["--description"].Any())
                description = argController.getKeyVal()["--description"].First();

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
            commentAction(boardName, listName);
            getCards(false);
        }

        /// <summary>
        /// If --description is inkl. and you are not creating a new card, then this is run.
        /// It will make sure the board, list and card is selected,
        /// initialize the needed controllers and let the card controller create the description.
        /// </summary>
        /// <param name="keyVal">The KeyVal args from ToLookup</param>
        private static void createDescription(IGrouping<string, string> keyVal)
        {
            // Initialize controllers and validate that the user-inputs are available in their respectful lists
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);
            
            if (!isCard)
                return;
            
            Ascii.clkCard();
            
            caController.createDescription(keyVal.FirstOrDefault(), cardId);

            // Print out some info nice for this selection
            Console.WriteLine("Created description: " + keyVal.FirstOrDefault());
            commentAction(boardName, listName, cardName);
            getCard(false);
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
            
            Ascii.clkCard();
            
            // Loop through and create each comment
            foreach (var val in keyVal)
            {
                if (val.Equals(""))
                    continue;
                
                caController.createComment(val, cardId);
                Console.WriteLine("Created comment: " + val);
            }
            
            // Print some good info for this selection
            commentAction(boardName, listName, cardName);
            getCard(false);
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
            
            Ascii.clkCard();
            
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
            commentAction(boardName, listName, cardName);
            getCard(false);
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
            
            Ascii.clkCard();

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
            
            // Print out some dope information!
            commentAction(boardName, listName, cardName, checklistName);
            getCard(false);
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

            if (isCard && argController.check >= 0
                       && Validators.inList(caController.getChecklists(cardId), argController.check))
            {
                isCheck = true;
                checkId = ObjectValues.getValueFromList(caController.getChecklists(cardId), argController.check, "id");
                checkName = ObjectValues.getValueFromList(caController.getChecklists(cardId), argController.check, "name");
            }
        }
        
        #endregion

        #region "View" methods - For displaying information in the terminal

        /// <summary>
        /// THis is used to print out a nice finish in the bottom of each "view".
        /// It is called from the create methods in this document.
        /// </summary>
        /// <param name="boardName">The board name to print out</param>
        /// <param name="listName">The list name to print out</param>
        /// <param name="cardName">The card name to print out</param>
        /// <param name="checklistName">The checklist name to print out</param>
        private static void commentAction(string boardName="", 
            string listName="", 
            string cardName="", 
            string checklistName="")
        {
            if (!checklistName.Equals(""))
                Console.WriteLine("In checklist : " + checklistName.Trim());
            if (!cardName.Equals(""))
                Console.WriteLine("In card      : " + cardName.Trim());
            if (!listName.Equals(""))
                Console.WriteLine("In list      : " + listName.Trim());
            if (!boardName.Equals(""))
                Console.WriteLine("In board     : " + boardName.Trim());
            Console.WriteLine();
        }

        /// <summary>
        /// This is used to print out common information in your current location (Board, List & Card)
        /// This is called in the get methods within this document
        /// </summary>
        /// <param name="boardName"></param>
        /// <param name="listName"></param>
        /// <param name="cardName"></param>
        private static void commentDestination(string boardName="", 
            string listName="", 
            string cardName="")
        {
            if (!boardName.Equals(""))
                Console.WriteLine("["+ ++argController.board +"] Board    : " + boardName);
            if (!listName.Equals(""))
                Console.WriteLine("["+ ++argController.list +"] List     : " + listName);
            if (!cardName.Equals(""))
                Console.WriteLine("["+ ++argController.card +"] Card     : " + cardName);
            Console.WriteLine();
        }
        

        #endregion

        /// <summary>
        /// If --new-profile is incl. this will run.
        /// It will call the REST interface to create the profile.
        /// </summary>
        /// <param name="keyVal">ToLookup args from argController</param>
        private static void createProfile(IGrouping<string, string> keyVal)
        {
            // Make sure we have a password
            if (!argController.getKeyVal()["--password"].Any())
                return;

            // Hash the password
            string pwHash = Resources.Random.hashString(argController.getKeyVal()["--password"].First());

            Profile p = new Profile();
            p.email = keyVal.First();
            p.password = pwHash;

            RestClient rest = new RestClient(restUrl);
            rest.post(p, "profile/create");
        }

        /// <summary>
        /// If --login is incl. this wil run.
        /// It will fetch the corresponding profile,
        /// from the database, to the provided values.
        /// If profile was not found, the user will not login.
        /// </summary>
        /// <param name="keyVal">From ToLookup method in argController</param>
        private static void login(IGrouping<string, string> keyVal)
        {
            // Make sure we have a password
            if (!argController.getKeyVal()["--password"].Any())
                return;

            // Hash the password
            string pwHash = Resources.Random.hashString(argController.getKeyVal()["--password"].First());

            string email = keyVal.First();

            user = new Profile();
            user.email = email;
            user.password = pwHash;
            
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

        private static void saveBoard()
        {
            iniOvController();
            iniLiController(boardId);
            iniCaController(listId);

            Console.WriteLine(boardId);
            Console.WriteLine(listId);
            
            //TODO: loop through lists
            // In that loop, loop through all cards,
            // in that loop, loop through all comments and checklists,
            // in that loop, loop through all checklist points
            // wat.. Use the FindAll() of course. bobobob, how to do this smartest.

            // Gather the information needed for the http request
            Board b = ovController.boards.Find(x => x.id == boardId);
            List<List> lists = liController.getLists();
            List<Card> cards = caController.getCards();
            List<Checklist> checklists = caController.getChecklists(cardId);
            List<ChecklistPoint> points = caController.getChecklistPoints(checkId);
            List<Comment> comments = caController.getComments(cardId);

            List<object> toQuery = new List<object>();
            toQuery.Add(b);
            toQuery.Add(lists);
            toQuery.Add(cards);
            toQuery.Add(checklists);
            toQuery.Add(points);
            toQuery.Add(comments);
            toQuery.Add(user.id);
            toQuery.Add(user.password);

            RestClient rest = new RestClient(restUrl);
            Console.WriteLine(rest.post(toQuery, "board/save"));
        }
    }
}