using System;
using clk.Resources;

namespace clk.Models
{
    public class Controls
    {

        public int xPos { get;  set; }
        public int yPos { get;  set; }

        public Controls()
        {
            xPos = 0;
            yPos = 0;
        }
        

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
                return 1;

            // Quit
            if (keyPush == ConsoleKey.Q)
            {
                Console.WriteLine();
                Console.WriteLine("You will be missed. Are you sure you wanna quit? ");
                if (EyeCandy.confirm())
                    Environment.Exit(0);
            }
            
            if (keyPush == ConsoleKey.B)
                goBack();

            return -1;
        }

        private void goBack()
        {
            if (Program.isCard)
            {
                Program.cardNum = -1;
                Program.isCard = false;
            }
            else if (Program.isBoard)
            {
                Program.listNum = -1;
                Program.boardNum = -1;
                Program.isList = false;
                Program.isBoard = false;
            }
        }
        
    }
}