using System.Collections.Generic;
using System.Net.Configuration;

namespace clk.Resources
{
    public static class Validators
    {
        
        /// <summary>
        /// This method will take a string value,
        /// and check if it is in fact a numeric value.
        /// </summary>
        /// <param name="value">The string value to check</param>
        /// <returns>True/False if the string actually is an integer</returns>
        public static bool isInt(string value)
        {
            int control;
            if (int.TryParse(value, out control))
                return true;
            return false;
        }

        /// <summary>
        /// This will take any list and check if a supplied position exists.
        /// This is used to check, that the user inputs actually exists in the
        /// available lists.
        /// </summary>
        /// <param name="list">A list of generic objects</param>
        /// <param name="position">The position to look up</param>
        /// <typeparam name="T">List of Board, List, Card etc.</typeparam>
        /// <returns>True/False if position is available</returns>
        public static bool inList<T>(List<T> list, int position)
        {
            if (position >= 0 && position < list.Count)
                return true;
            return false;
        }
    }
}