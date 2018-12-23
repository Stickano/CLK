using System;
using System.Collections.Generic;
using clk.Models;
using clk.Resources;

namespace clk.Views
{
    public class ContinuousWrite
    {

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
                if (xPos == i)
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
                foreach (Card card in cardsInList)
                {
                    string name = "  |  " + EyeCandy.subString(card.name, 24);
                    int nameLength = name.Length;

                    Console.WriteLine();
                    Console.Write(EyeCandy.indent(indent + 2) + "|" + EyeCandy.indent(28) + "|");
                    
                    Console.WriteLine();
                    Console.Write(EyeCandy.indent(indent) + name);
                    Console.Write(EyeCandy.indent(30-nameLength) + " |");

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

        private void cursor(int indent = 0)
        {
            EyeCandy.color("cyan");
            Console.Write(EyeCandy.indent(indent) + "> ");
            EyeCandy.reset();
        }


        public void boxTop(int boxSize)
        {
            Console.Write(EyeCandy.indent(2) + "╔");
            for (int i = 0; i < boxSize-2; i++)
            {
                Console.Write("═");
            }
            Console.Write("╗");
        }

        public void boxBottom(int boxSize)
        {
            Console.Write(EyeCandy.indent(2) + "╚");
            for (int i = 0; i < boxSize-2; i++)
            {
                Console.Write("═");
            }
            Console.Write("╝");
        }
    }
}