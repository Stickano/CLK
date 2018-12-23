using System;
using System.CodeDom.Compiler;

namespace clk.Resources
{
    public static class EyeCandy
    {
        /// <summary>
        /// A method to indent text in the terminal window.
        /// </summary>
        /// <param name="spaces">The amount of space character(s) to indent</param>
        /// <returns>A string of spaces</returns>
        public static string indent(int spaces)
        {
            return "".PadLeft(spaces);
        }

        /// <summary>
        /// Returns a console color, which is used to display the
        /// various elements, in various colors. 
        /// </summary>
        /// <param name="color">The ConsoleColor to return: red, blue, gray, green, yellow</param>
        /// <returns>The select console color</returns>
        public static void color(string color = "red")
        {
            ConsoleColor c = ConsoleColor.Red;
            if (color.Equals("blue"))
                c = ConsoleColor.Blue;
            if (color.Equals("cyan"))
                c = ConsoleColor.Cyan;
            if (color.Equals("green"))
                c = ConsoleColor.Green;
            if (color.Equals("yellow"))
                c = ConsoleColor.Yellow;
            
            Console.ForegroundColor = c;
        }

        /// <summary>
        /// Resets the colors after the point it is called.
        /// (Call a color, call reset after the color has been used)
        /// </summary>
        public static void reset()
        {
            Console.ResetColor();
        }

        /// <summary>
        /// This will shorten a text down, if longer than the line length,
        /// and add a couple of dots to show that the line continuous. 
        /// </summary>
        /// <param name="text">The text to shorten (if to long)</param>
        /// <param name="lineLength">The length of the line (amount of characters the text can take up)</param>
        /// <returns>The shortened text</returns>
        public static string subString(string text, int lineLength)
        {
            if (text.Length > lineLength)
                text = text.Substring(0, lineLength - 2) + "..";
            return text;
        }
        
        /// <summary>
        /// This will ask a yes/no question,
        /// and return true/false if yes or no.
        /// </summary>
        /// <returns>True/false if selected yes (or empty)</returns>
        public static bool confirm()
        {
            Console.Write("Yes/no: ");
            string answer = Console.ReadLine();
            Console.WriteLine();
            if (answer.Equals("")
                || answer.Substring(0, 1).ToLower().Equals("y"))
                return true;
            return false;
        }
    }
}