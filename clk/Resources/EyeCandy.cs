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
    }
}