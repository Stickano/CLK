using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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
                    cursor();
                else
                    indent = 2;
                
                Console.Write(EyeCandy.indent(indent) + board.name);
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
        public void writeLists(List<List> lists, int startPos, int xPos, int yPos, List<Card> cardsInList = null)
        {
            int listsCount = lists.Count;
            int toDisplay = 3;
            if (listsCount < toDisplay)
                toDisplay = listsCount;

            // Top of boxes
            Console.WriteLine();            
            for (int i = 0; i < toDisplay; i++)
            {
                boxTop(30);   
            }

            // Headlines / names
            Console.WriteLine();
            for (int i = startPos; i < toDisplay; i++)
            {
                string name = EyeCandy.subString(lists[startPos + i].name, 28);
                int nameLength = name.Length;
                int indent = 0;
                if (xPos == i && yPos == 0)
                    cursor(2);
                else
                    indent = 4;
                
                Console.Write(EyeCandy.indent(indent) + name);
                Console.Write(EyeCandy.indent(30 - nameLength - 2));
            }

            // Bottom of boxes
            Console.WriteLine();
            for (int i = 0; i < toDisplay; i++)
            {
                boxBottom(30);   
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
                        cursor();
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
        /// The cursor used to inform the user where he is on the board.
        /// </summary>
        /// <param name="indent">Characters to indent (if you want)</param>
        private void cursor(int indent = 0)
        {
            EyeCandy.color("cyan");
            Console.Write(EyeCandy.indent(indent) + "> ");
            EyeCandy.reset();
        }


        /// <summary>
        /// Used to print out the list headlines.
        /// Prints out the top of a box-ish kinda thing.
        /// </summary>
        /// <param name="boxSize">The size of the box (character spaces)</param>
        public void boxTop(int boxSize)
        {
            Console.Write(EyeCandy.indent(2) + "╔");
            for (int i = 0; i < boxSize-2; i++)
            {
                Console.Write("═");
            }
            Console.Write("╗");
        }

        /// <summary>
        /// Used to print out the bottom of the list headlines.
        /// Prints outthe bottom of the box thingy.
        /// </summary>
        /// <param name="boxSize">The size of the box.</param>
        public void boxBottom(int boxSize)
        {
            Console.Write(EyeCandy.indent(2) + "╚");
            for (int i = 0; i < boxSize-2; i++)
            {
                Console.Write("═");
            }
            Console.Write("╝");
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
            boxTop(nameLength + 4);
            Console.WriteLine(EyeCandy.indent(2));
            
            int indent = 2;
            if (yPos == 0)
                cursor();
            else
                indent = 4;
            
            Console.Write(EyeCandy.indent(indent) + cardName);
            Console.WriteLine();
            boxBottom(nameLength + 4);
            Console.WriteLine();

            indent = 0;
            if (yPos == 1)
                cursor();
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
                    cursor();
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
                    cursor();
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
                        cursor();
                    else
                        indent = 2;
                    
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
                    cursor();
                else
                    indent = 2;
                
                Console.WriteLine(EyeCandy.indent(indent) + comment.created);
                Console.WriteLine(EyeCandy.indent(2) + comment.comment);
                Console.WriteLine();
            }
        }
    }
}