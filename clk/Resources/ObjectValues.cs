using System;
using System.Collections.Generic;

namespace clk.Resources
{
    public static class ObjectValues
    {
        
        /// <summary>
        /// This method will return the ID from the selected element (board, list, card etc)
        /// </summary>
        /// <param name="list">The list to pick an element from</param>
        /// <param name="position">The position that the element is stored in the list</param>
        /// <param name="search">The keyword to search for (i.e. id, name etc)</param>
        /// <typeparam name="T">Board, List, Card etc (Generic)</typeparam>
        /// <returns>The ID for the selected element</returns>
        public static string getValueFromList<T>(List<T> list, int position, string search)
        {
            int searchLen = search.Length;
            
            string select = list[position].ToString();
            string[] split = select.Split(',');
            string id = Array.Find(split, s => s.Trim().Substring(0, searchLen).Equals(search));
            
            return id.Substring(searchLen + 2);
        }
    }
}