using System;
using System.Collections.Generic;
using System.Linq;
using clk.Controllers;
using clk.Models;
using clk.Resources;
using clk.Views;

namespace clk
{
    internal class Continuous : Program
    {
        private Controls controls;
        private ContinuousWrite write;
        
        public Continuous()
        {
            controls = new Controls();
            write = new ContinuousWrite();
            iniOvController();
            Console.CursorVisible = false;
        }

        public void loopApp()
        {
            
            

            while (true)
            {
                
                Console.Clear();

                // Write top logo
                if (isCard)
                    Ascii.clkCard();
                else if (isBoard)
                    Ascii.clkBoard();
                else 
                    Ascii.clkBoards();
                /*else
                    Ascii.clk();*/


                int maxPos = 0;

                // If the BOARD is not set, display ALL BOARDS
                if (!isBoard)
                {
                    write.writeBoards(ovController.getBoards(), controls.yPos);
                    maxPos = ovController.getBoards().Count;
                }

                
                // If BOARD is set, display LISTS
                if (isBoard)
                {
                    iniLiController(boardId);
                    maxPos = liController.getLists().Count;
                    listNum = controls.yPos;

                    // If you moved down, initialise and update controllers 
                    if (controls.yPos > 0)
                    {
                        listNum = controls.yPos;
                        iniLiController(boardId);
                        iniCaController(listId);
                    }

                    // Write out the board name that we are working in
                    Console.WriteLine();
                    EyeCandy.color("yellow");
                    Console.WriteLine("  -> "+ boardName);
                    EyeCandy.reset();

                    
                    int startPos = 0;
                    if (controls.xPos - 3 > 0)
                        startPos = controls.xPos - 3;

                    List<Card> cardsToRead = null;
                    if (controls.yPos > 0)
                        cardsToRead = caController.getCards();
                    
                    write.writeLists(liController.getLists(), startPos, controls.xPos, controls.yPos, cardsToRead);
                }

                
                // Ask for user input
                Console.WriteLine();
                Console.WriteLine();
                ConsoleKey answer = Console.ReadKey().Key;

                // If the user just navigated (x and y pos in controls)
                // Continue to reload the user interface.
                if (controls.cursorAction(answer, maxPos) == -1)
                    continue;

                // If we selected enter without the board is set,
                // Select a board
                if (!isBoard)
                {
                    boardNum = controls.yPos;
                    iniOvController();
                }

                if (!isList)
                {
                    listNum = controls.yPos - 1;
                }

            }
            
        }
        
        
        
        
    }
}