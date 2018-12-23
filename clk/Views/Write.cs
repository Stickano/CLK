using System;
using System.Collections.Generic;
using System.Linq;
using clk.Controllers;
using clk.Models;
using clk.Resources;

namespace clk.Views
{
    public class Write
    {
        
        /// <summary>
        /// Print out all available boards (overview)
        /// </summary>
        /// <param name="boards">The list of all boards (overview controller</param>
        public void allBoards(List<Board> boards)
        {
            int br = 0;
            Console.WriteLine("Available boards:");

            if (boards.Count == 0)
            {
                EyeCandy.color("yellow");
                Console.WriteLine(EyeCandy.indent(6) + "[ Use --new-board to add a new board ]");
                EyeCandy.reset();
            }
            
            foreach (Board board in boards)
            {
                br++;
                Console.WriteLine("  ["+ br +"] " + board.name);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Print out all available lists in a board
        /// </summary>
        /// <param name="lists">The list of lists to print out</param>
        public void allLists(List<List> lists)
        {
            Console.WriteLine("Available lists:");

            if (lists.Count == 0)
            {
                EyeCandy.color("yellow");
                Console.WriteLine(EyeCandy.indent(6) + "[ Use --new-list to add a new list ]");
                EyeCandy.reset();
            }
            
            int br = 0;
            foreach (List list in lists)
            {
                br++;
                Console.WriteLine("  ["+ br +"] " + list.name);
            }

            Console.WriteLine();        
        }

        /// <summary>
        /// Print out all available cards in a list
        /// </summary>
        /// <param name="cards"></param>
        public void allCards(List<Card> cards)
        {
            Console.WriteLine("Available cards:");

            if (cards.Count == 0)
            {
                EyeCandy.color("yellow");
                Console.WriteLine(EyeCandy.indent(6) + "[ Use --new-card to add a new card ]");
                EyeCandy.reset();
            }
            
            int br = 0;
            foreach (Card card in cards)
            {
                
                br++;
                string cardCount = "  [" + br + "] ";
                int cardCountLen = cardCount.Length;

                Console.Write(cardCount);
                
                if (card.label != null)
                    EyeCandy.color(card.label.label);
                
                Console.WriteLine(card.name);
                EyeCandy.reset();

                if(card.description != null && !card.description.Equals(""))
                    Console.WriteLine(EyeCandy.indent(cardCountLen) + card.description);
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Used to write the headline (name) of a card.
        /// (Inside a box kinda thing).
        /// The length of the box will match the length of the name.
        /// </summary>
        /// <param name="headline">The text to print inside the box</param>
        public void cardHeadline(string headline)
        {
            int hLen = headline.Length + 2;
            
            Console.Write(EyeCandy.indent(4) + "╔");
            for (int i = 0; i < hLen; i++)
            {
                Console.Write("═");
            }
            Console.Write("╗");

            Console.WriteLine();
            EyeCandy.color("yellow");
            Console.Write(EyeCandy.indent(6) + headline);
            EyeCandy.reset();

            Console.WriteLine();
            Console.Write(EyeCandy.indent(4) + "╚");
            for (int i = 0; i < hLen; i++)
            {
                Console.Write("═");
            }
            Console.Write("╝");
            Console.WriteLine();
        }

        /// <summary>
        /// Print out a card and its content
        /// </summary>
        /// <param name="card">The card to print out</param>
        public void card(Card card, CardController controller)
        {
            
            cardHeadline(card.name);
            Console.WriteLine();
            
            if (!card.description.Equals(""))
                Console.WriteLine(EyeCandy.indent(6) + card.description);
            else
            {
                EyeCandy.color("yellow");
                Console.WriteLine(EyeCandy.indent(6) + "[ Use --description to add a description ]");
                EyeCandy.reset();
            }
            
            Console.WriteLine();
            Console.WriteLine();

            if (controller.getChecklists(card.id).Count == 0)
            {
                EyeCandy.color("yellow");
                Console.WriteLine(EyeCandy.indent(6) + "[ Use --new-check to add a new checklist ]");
                EyeCandy.reset();
            }

            // Checklist section
            int br = 0;
            int brChk = 0;
            foreach (Checklist checklist in controller.getChecklists(card.id))
            {
                brChk++;
                Console.WriteLine("["+brChk+"] "+checklist.name);

                if (!controller.getChecklistPoints(checklist.id).Any())
                {
                    EyeCandy.color("yellow");
                    Console.WriteLine(EyeCandy.indent(6) + "[ Use --new-point to add a new checklist point ]");
                    EyeCandy.reset();
                }

                // Checklist points
                foreach (ChecklistPoint point in controller.getChecklistPoints(checklist.id))
                {
                    br++;
                    string outCountP = "[" + br + "] ";

                    // Colors, colors everywhere!
                    if (point.isCheck)
                        EyeCandy.color("green");
                    
                    Console.WriteLine(EyeCandy.indent(2) + outCountP + point.name);
                    EyeCandy.reset();
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            comments(controller.getComments(card.id));
        }

        /// <summary>
        /// Print out comments.
        /// Called from the card() method i.e.
        /// </summary>
        /// <param name="comments">The list of comments to print out</param>
        public void comments(List<Comment> comments)
        {
            int br = 0;
            Console.WriteLine();
            
            if (comments.Count > 0)
                Console.WriteLine("Comment(s):");
            else
            {
                EyeCandy.color("yellow");
                Console.WriteLine(EyeCandy.indent(6) + "[ Use --comment to add a new comment ]");
                EyeCandy.reset();
            }
            
            foreach (Comment comment in comments)
            {
                br++;
                string outCount = "  [" + br + "] ";
                int outCountLen = outCount.Length;
                Console.WriteLine(outCount + comment.created);
                Console.WriteLine(EyeCandy.indent(outCountLen) + comment.comment);
                Console.WriteLine();
            }
        }
        
        /// <summary>
        /// If an error has been encountered on the way,
        /// send it to this method - It will print it and exit the program.
        /// </summary>
        /// <param name="message">The error message to print out before exiting</param>
        public void error(string message)
        {
            Console.WriteLine();
            Console.Write("Error: ");
            EyeCandy.color();
            Console.Write(message);
            Console.WriteLine();
            Environment.Exit(0);
        }
        
        /// <summary>
        /// THis is used to print out a nice finish in the bottom of each "view".
        /// It is called from the create methods in this document.
        /// </summary>
        /// <param name="boardName">The board name to print out</param>
        /// <param name="listName">The list name to print out</param>
        /// <param name="cardName">The card name to print out</param>
        /// <param name="checklistName">The checklist name to print out</param>
        public static void commentAction(string boardName="", 
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
        public void commentDestination()
        {
            Console.WriteLine();
            if (Program.boardName != null)
                Console.WriteLine("In location:");
            
            if (Program.cardName != null)
            {
                Console.Write("["+ ++Program.cardNum +"] ");
                EyeCandy.color("green");
                Console.Write(Program.cardName);
                EyeCandy.reset();
                Console.WriteLine();
            }
            
            if (Program.listName != null)
            {
                Console.Write("["+ ++Program.listNum +"] ");
                EyeCandy.color("green"); 
                Console.Write(Program.listName);
                EyeCandy.reset();
                Console.WriteLine();
            }

            if (Program.boardName != null)
            {
                Console.Write("["+ ++Program.boardNum +"] ");
                EyeCandy.color("green"); //red
                Console.Write(Program.boardName);
                EyeCandy.reset();
                Console.WriteLine();
            }
        }

        /// <summary>
        /// If a user wants to update an element,
        /// first run this, display info and ask to confirm.
        /// </summary>
        /// <param name="from">The name to change from</param>
        /// <param name="to">The name to change to</param>
        /// <returns>True/False if the user selected yes (or empty)</returns>
        public bool confirmUpdate(string from, string to)
        {
            Console.WriteLine("Change: " + from);
            Console.WriteLine("To: " + to + "?");

            return EyeCandy.confirm();
        }

        /// <summary>
        /// When deleting, this will display info, then confirm.
        /// </summary>
        /// <param name="name">The name of the item you're about to delete.</param>
        /// <returns>True/false if user selected yas or no</returns>
        public bool confirmDelete(string name)
        {
            Console.WriteLine("Delete: " + name + "?");
            return EyeCandy.confirm();
        }

        

    }
}