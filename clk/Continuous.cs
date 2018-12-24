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


                int xMaxPos = 0;
                int yMaxPos = 0;

                // If the BOARD is not set, display ALL BOARDS
                if (!isBoard)
                {
                    write.writeBoards(ovController.getBoards(), controls.yPos);
                    yMaxPos = ovController.getBoards().Count;
                }

                
                // If BOARD is set, display LISTS
                if (isBoard && !isCard)
                {
                    listNum = controls.xPos;
                    iniLiController(boardId);
                    xMaxPos = liController.getLists().Count;
                    
                    iniCaController(listId);
                    yMaxPos = caController.getCards().Count + 1; // +1 for the list line

                    // Write out the board name that we are working in
                    Console.WriteLine();
                    EyeCandy.color("yellow");
                    Console.WriteLine("  -> "+ boardName);
                    EyeCandy.reset();

                    // If you change list, make sure yPos is within limits
                    if (controls.yPos >= yMaxPos)
                        controls.yPos--;

                    
                    int startPos = 0;
                    if (controls.xPos - 3 > 0)
                        startPos = controls.xPos - 3;

                    // Write out the lists, and the cards associated to the current selected list
                    List<Card> cardsToRead = caController.getCards();
                    write.writeLists(liController.getLists(), startPos, controls.xPos, controls.yPos, cardsToRead);
                }
                
                
                // If CARD is set, display its content
                if (isCard && isList)
                {
                    Card card = caController.cards.Find(x => x.id == cardId);

                    int chkCount = caController.getChecklists(cardId).Count;
                    int pointCount = caController.getChecklistPointsInCard(cardId).Count;
                    int commentCount = caController.getComments(cardId).Count;

                    yMaxPos = 2 + chkCount + pointCount + commentCount; // +2 for headline and description
                    
                    write.writeCard(card, caController, controls.yPos, yMaxPos);
                }

                
                // Ask for user input
                Console.WriteLine();
                Console.WriteLine();
                ConsoleKey answer = Console.ReadKey().Key;

                // If the user just navigated (x and y pos in controls)
                // Continue to reload the user interface.
                if (controls.cursorAction(answer, xMaxPos, yMaxPos) == -1)
                    continue;

                // If we selected enter without the board is set,
                // Select a board
                if (!isBoard)
                {
                    boardNum = controls.yPos;
                    iniOvController();
                }

                // If the user select a card, initialize the card controller
                if (isList && !isCard && controls.yPos != 0)
                {
                    cardNum = controls.yPos - 1; // -1 to correct index (top (0) is list headline)
                    iniCaController(listId);
                    controls.yPos = 1; // Start at description
                }
            }
        }
        
        
        
    }
}