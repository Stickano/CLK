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
    }
}