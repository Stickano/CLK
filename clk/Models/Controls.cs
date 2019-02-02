using System;
using System.ComponentModel;
using clk.Resources;

namespace clk.Models
{
    public class Controls
    {

        public int xPos { get;  set; }
        public int yPos { get;  set; }

        private int prevXPos;
        private int prevYPos;

        public Controls()
        {
            xPos = 0;
            yPos = 0;
            prevXPos = 0;
            prevYPos = 0;
        }
        
        /// <summary>
        /// On user input, this is run.
        /// It will determine the user input and the do associated action.
        /// It will set the value for x and y axis (which item the user resides on)
        /// and to do that properly, it has to know where the limit for the x and y axis is.
        /// The limit (xMaxPos and yMaxPos) is the count of items on the y and x axis.
        /// </summary>
        /// <param name="keyPush">The user input (push key)</param>
        /// <param name="xMaxPos">The max position on the x axis</param>
        /// <param name="yMaxPos">The max position on the y axis</param>
        /// <returns>
        /// -1 if navigation,
        /// 1 if select,
        /// 2 if edit,
        /// 3 if delete
        /// 4 if create 1 (i)
        /// 5 if create 2 (o)
        /// </returns>
        public int cursorAction(ConsoleKey keyPush, int xMaxPos, int yMaxPos)
        {

            #region up, down, left, right navigation.
    
            // move left
            if (keyPush == ConsoleKey.LeftArrow || keyPush == ConsoleKey.H)
            {
                if (xPos - 1 >= 0)
                    xPos--;
            }
            
            // move right
            if (keyPush == ConsoleKey.RightArrow || keyPush == ConsoleKey.L)
            {
                if (xPos +1 < xMaxPos)
                    xPos++;
            }
            
            // move up
            if (keyPush == ConsoleKey.UpArrow || keyPush == ConsoleKey.K)
            {
                if (yPos -1 >= 0)
                    yPos--;
            }
            
            // move down
            if (keyPush == ConsoleKey.DownArrow || keyPush == ConsoleKey.J)
            {
                if (yPos +1 < yMaxPos)
                    yPos++;
            }

            #endregion

            if (keyPush == ConsoleKey.Enter || keyPush == ConsoleKey.Spacebar)
            {
                prevXPos = xPos;
                prevYPos = yPos;
                return 1;
            }

            // Quit
            if (keyPush == ConsoleKey.Q)
            {
                Console.WriteLine();
                Console.WriteLine("You will be missed. Are you sure you wanna quit? ");
                if (EyeCandy.confirm())
                    Environment.Exit(0);
            }
            
            // Navigate one step back
            if (keyPush == ConsoleKey.B)
                goBack();

            #region Additional navigations. An additional Christmas present.

            if (keyPush == ConsoleKey.R)
                xPos = 0;

            if (keyPush == ConsoleKey.T)
                xPos = xMaxPos - 1;

            if (keyPush == ConsoleKey.F)
                yPos = 0;

            if (keyPush == ConsoleKey.G)
                yPos = yMaxPos - 1;

            #endregion

            if (keyPush == ConsoleKey.U)
                return 2;

            if (keyPush == ConsoleKey.D)
                return 3;

            if (keyPush == ConsoleKey.I)
                return 4;

            if (keyPush == ConsoleKey.O)
                return 5;

            if (keyPush == ConsoleKey.S)
                return 6;

            /*if (xPos >= xMaxPos && xPos > 0)
                xPos--;
            if (yPos >= yMaxPos)
                yPos--;*/

            return -1;
        }

        /// <summary>
        /// Go one step back from where the user current operates.
        /// </summary>
        private void goBack()
        {
            yPos = prevYPos;
            xPos = prevXPos;
            Program.isSettings = false;
            
            if (Program.isCard)
            {
                Program.cardNum = -1;
                Program.isCard = false;
            }
            else if (Program.isBoard)
            {
                yPos = Program.boardNum;
                xPos = 0;
                
                Program.listNum = -1;
                Program.boardNum = -1;
                Program.listId = null;
                Program.boardId = null;
                Program.isList = false;
                Program.isBoard = false;
            }
        }
        
    }
}