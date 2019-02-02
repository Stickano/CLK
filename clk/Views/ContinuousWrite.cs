using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using clk.Controllers;
using clk.Models;
using clk.Resources;

namespace clk.Views
{
    public class ContinuousWrite
    {

        /// <summary>
        /// Write out all the boards
        /// </summary>
        /// <param name="boards">A list of Board (the boards to print)</param>
        /// <param name="linePosition">yPos from controls(user location)</param>
        public void writeBoards(List<Board> boards, int linePosition)
        {
            int br = 0;
            Console.WriteLine("Available boards:");
            foreach (Board board in boards)
            {
                
                int indent = 0;
                if (br == linePosition)
                    EyeCandy.cursor();
                else
                    indent = 2;
                
                Console.Write(EyeCandy.indent(indent) + board.name);
                Console.WriteLine();
                br++;
            }
        }

        /// <summary>
        /// Write out lists in a board. This is a bit.. well.
        /// TODO: could maybe load all card for a board and display em, instead of the drop down boxes. 
        /// </summary>
        /// <param name="lists">All the lists to print out</param>
        /// <param name="startPos">3 lists are shown, this is the first position to display</param>
        /// <param name="xPos">X position from controls</param>
        /// <param name="yPos">Y position from controls</param>
        /// <param name="cardsInList">All the cards associated to the list at x-position</param>
        public void writeList(List<List> lists, int xPos, int yPos, int startPos, List<Card> cardsInList = null)
        //public void writeLists(List<List> lists, int startPos, int xPos, int yPos, List<Card> cardsInList = null)
        {
            int listsCount = lists.Count;
            int toDisplay = 3;
            if (listsCount < toDisplay)
                toDisplay = listsCount;

            /*Console.WriteLine("xpos: "+xPos);
            Console.WriteLine("todisplay: "+toDisplay);
            //Console.WriteLine("start: "+startPos);
            Console.WriteLine("count: " + listsCount);*/
            
            // Top of boxes
            Console.WriteLine();            
            for (int i = 0; i < toDisplay; i++)
            {
                EyeCandy.boxTop(30);   
            }

            // Headlines / names
            Console.WriteLine();
            int listBr = 0;
            foreach (List list in lists)
            {
                string name = EyeCandy.subString(list.name, 28);
                int nameLength = name.Length; // kek, I set the length in the line above. Good job jeppesen.. Good job indeed.
                int indent = 0;
                if (startPos+listBr == xPos  && yPos == 0)
                    EyeCandy.cursor(2);
                else
                    indent = 4;

                listBr++;
                Console.Write(EyeCandy.indent(indent) + name);
                Console.Write(EyeCandy.indent(30 - nameLength - 2));
            }
            
            /*for (int i = 0; i < toDisplay; i++)
            {
                string name = EyeCandy.subString(lists[xPos].name, 28);
                int nameLength = name.Length; // kek, I set the length in the line above. Good job jeppesen.. Good job indeed.
                int indent = 0;
                if (xPos -startPos == i  && yPos == 0)
                    cursor(2);
                else
                    indent = 4;
                
                Console.Write(EyeCandy.indent(indent) + name);
                Console.Write(EyeCandy.indent(30 - nameLength - 2));
            }*/

            // Bottom of boxes
            Console.WriteLine();
            for (int i = 0; i < toDisplay; i++)
            {
                EyeCandy.boxBottom(30);   
            }
            
            
            // Cards
            int column = xPos - startPos;
            if (cardsInList != null)
            {
                int indent = column * 32;
                int br = 0;
                foreach (Card card in cardsInList)
                {

                    br++;
                    
                    // Top of card box 
                    Console.WriteLine();
                    Console.Write(EyeCandy.indent(indent + 2) + "|" + EyeCandy.indent(28) + "|");
                    
                    string name = EyeCandy.subString(card.name, 24);
                    int nameLength = name.Length;
                    
                    // Left side of box with name line
                    Console.WriteLine();
                    Console.Write(EyeCandy.indent(indent + 2) + "|");

                    // To write cursor or not to write cursor, that is the question
                    int ind = 0;
                    if (yPos == br)
                        EyeCandy.cursor();
                    else
                        ind = 2;
                    
                    // Print the name of the card, and the end of the box with the name line (right side)
                    Console.Write(EyeCandy.indent(ind) + name);
                    Console.Write(EyeCandy.indent(30-nameLength - 4) + "|");

                    // Bottom of the card box
                    Console.WriteLine();
                    Console.Write(EyeCandy.indent(indent + 2) + "|");
                    for (int i = 0; i < 14; i++)
                    {
                        Console.Write("_ ");
                    }
                    Console.Write("|");
                }
            }
        }
        
        /// <summary>
        /// Print out a card and its content
        /// </summary>
        /// <param name="card">The card to print out</param>
        public void writeCard(Card card, CardController controller, int yPos, int yMaxPos)
        {

            string cardName = card.name;
            int nameLength = cardName.Length;

            Console.WriteLine();
            EyeCandy.boxTop(nameLength + 4);
            Console.WriteLine(EyeCandy.indent(2));
            
            int indent = 2;
            if (yPos == 0)
                EyeCandy.cursor();
            else
                indent = 4;
            
            Console.Write(EyeCandy.indent(indent) + cardName);
            Console.WriteLine();
            EyeCandy.boxBottom(nameLength + 4);
            Console.WriteLine();

            indent = 0;
            if (yPos == 1)
                EyeCandy.cursor();
            else
                indent = 2;
            
            if (!card.description.Equals(""))
                Console.WriteLine(EyeCandy.indent(indent) + card.description);
            else
            {
                EyeCandy.color("yellow");
                Console.WriteLine(EyeCandy.indent(indent) + "[ No description ]");
                EyeCandy.reset();
            }
            
            Console.WriteLine();
            Console.WriteLine();

            if (controller.getChecklists(card.id).Count == 0)
            {
                
                indent = 0;
                if (yPos == 2)
                    EyeCandy.cursor();
                else
                    indent = 2;
                
                EyeCandy.color("yellow");
                Console.WriteLine(EyeCandy.indent(indent) + "[ No checklists ]");
                EyeCandy.reset();
            }

            // Checklist section
            int br = 0;
            int brChk = 1;
            foreach (Checklist checklist in controller.getChecklists(card.id))
            {   
                brChk++;
                
                
                indent = 0;
                if (yPos == brChk + br)
                    EyeCandy.cursor();
                else
                    indent = 2;
                
                Console.WriteLine(EyeCandy.indent(indent) + checklist.name);

                if (!controller.getChecklistPoints(checklist.id).Any())
                {
                    EyeCandy.color("yellow");
                    Console.WriteLine(EyeCandy.indent(4) + "[ No checklist points ]");
                    EyeCandy.reset();
                }

                // Checklist points
                foreach (ChecklistPoint point in controller.getChecklistPoints(checklist.id))
                {
                    br++;
                    
                    indent = 0;
                    if (yPos == brChk + br)
                        EyeCandy.cursor(2);
                    else
                        indent = 4;
                    
                    // Colors, colors everywhere!
                    if (point.isCheck)
                        EyeCandy.color("green");
                    
                    Console.WriteLine(EyeCandy.indent(indent) + point.name);
                    EyeCandy.reset();
                }

                Console.WriteLine();
            }

            int ySoFar = brChk + br;

            Console.WriteLine();
            comments(controller.getComments(card.id), ySoFar, yPos);
        }

        /// <summary>
        /// Print out comments.
        /// Called from the card() method i.e.
        /// </summary>
        /// <param name="comments">The list of comments to print out</param>
        public void comments(List<Comment> comments, int startPos, int yPos)
        {
            int br = 0;
            Console.WriteLine();
            
            if (comments.Count > 0)
                Console.WriteLine("Comment(s):");
            else
            {
                EyeCandy.color("yellow");
                Console.WriteLine(EyeCandy.indent(2) + "[ No comments ]");
                EyeCandy.reset();
            }
            
            foreach (Comment comment in comments)
            {
                br++;
                
                
                int indent = 0;
                if (yPos-startPos == br)
                    EyeCandy.cursor();
                else
                    indent = 2;
                
                Console.WriteLine(EyeCandy.indent(indent) + comment.created);
                Console.WriteLine(EyeCandy.indent(2) + comment.comment);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// This will write out the available options (the menu).
        /// It will display a dynamic menu depending on, if the board and card
        /// is set or not.
        /// </summary>
        /// <param name="board">If the board is set</param>
        /// <param name="card">if the card is set</param>
        public void writeMenu(bool board = false, bool card = false) 
        {
            Console.WriteLine();
            Console.WriteLine();

            // The lines available for the menu.
            string navigateInfo = "h/j/k/l  (or arrows)  to navigate.";
            string selectInfo = "Space/Enter to select the item at he cursor.";
            string checkPoint = "Space/Enter to check/un-check a checklist point";
            string navFirstList = "r/t    first/last list.";
            string navToTop = "f/g    top/bottom selection.";
            string newList = "i    create a new list.";
            string newCard = "o    create a new card.";
            string newBoard = "i    create a new board.";
            string updElem = "u    update element at cursor.";
            string delElem = "d    delete element at cursor.";
            string newCheck = "i    create a new checklist.";
            string newPoint = "o    create a new checklist point.";
            string back = "b    back one step.";
            string quit = "q    quit the application.";
            string boardDetails = "m    show board details.";
            string newCom = "c    create a new comment.";
            string settings = "s    show settings.";
            string login = "a    to login.";
            string cloudSave = "x    save board to cloud.";
            string cloudGet = "z    get all boards from the cloud.";
            
            // Write me this, pls.
            writeMenu(navigateInfo);

            // Write this if the board is NOT set
            if (!board)
            {
                writeMenu(selectInfo);
                writeMenu(newBoard);
                writeMenu(updElem);
                writeMenu(delElem);
            }

            // Write this if the bard is set, and the card is NOT set
            if (board && !card)
            {
                writeMenu(selectInfo);
                writeMenu(newList);
                writeMenu(newCard);
                writeMenu(updElem);
                writeMenu(delElem);

                writeMenu(navFirstList);
                writeMenu(navToTop);
            }

            // Write this if the card is set
            if (card)
            {
                writeMenu(checkPoint);
                writeMenu(newCheck);
                writeMenu(newPoint);
                writeMenu(newCom);
                
                writeMenu(updElem);
                writeMenu(delElem);
                writeMenu(navToTop);
            }

            // Write this if the board is set
            if (board)
            {
                writeMenu(boardDetails);
                writeMenu(back);
            }
            
            // Also write this.
            writeMenu(settings);
            writeMenu(quit);
            
            // TODO: missing cloud thingys
        }

        /// <summary>
        /// This will write out a line of the menu options
        /// </summary>
        /// <param name="toWrite">The line to write</param>
        private void writeMenu(string toWrite)
        {
            Console.WriteLine();
            Console.Write("[ " + toWrite);
            Console.Write(EyeCandy.indent(80 - toWrite.Length) + " ]");
        }

        /// <summary>
        /// This will ask for a new value, when the user decides to update a element.
        /// </summary>
        /// <param name="oldValue">The name of the old object</param>
        /// <returns>New objects name, or if empty just the old one.</returns>
        public string updateValue(string oldValue)
        {
            Console.WriteLine();
            Console.WriteLine("Update the value of: " + oldValue);
            Console.WriteLine("New value: ");

            string input = Console.ReadLine();
            if (input.Equals(""))
                return oldValue;
            
            return input;
        }

        /// <summary>
        /// This will ask for a new value for a new element.
        /// </summary>
        /// <param name="toCreate">What the user is creating (Board, List, Card, Checklist, Checklist Point, Comment)</param>
        /// <returns>The user input</returns>
        public string createValue(string toCreate)
        {
            Console.WriteLine();
            Console.WriteLine("Create a new " + toCreate + ": ");
            return Console.ReadLine();
        }

        public void showSettings(int yPos, string defaultBoard, string autoLogin, string autoPush)
        {
            Console.WriteLine("Available settings:");

            string first = "Default board set         : " + defaultBoard;
            string second = "Auto login                : " + autoLogin;
            string third = "Auto push changes to cloud: " + autoPush;
            
            string[] settings = {first, second, third};

            int br = 0;
            foreach (string s in settings)
            {
                int indent = 2;
                if (yPos == br)
                {
                    indent = 0;
                    EyeCandy.cursor();
                }

                Console.WriteLine(EyeCandy.indent(indent) + s);
                br++;
            }
        }
    }
}




