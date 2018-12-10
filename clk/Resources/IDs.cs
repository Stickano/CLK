using System;
using System.Collections.Generic;
using System.Linq;

namespace clk.Resources
{
    public static class IDs
    {
        
        /// <summary>
        /// This method will return the ID from the selected element (board, list, card etc)
        /// </summary>
        /// <param name="list">The list to pick an element from</param>
        /// <param name="position">The position that the element is stored in the list</param>
        /// <typeparam name="T">Board, List, Card etc</typeparam>
        /// <returns>The ID for the selected element</returns>
        public static string getIdFromList<T>(List<T> list, int position)
        {
            string select = list[position].ToString();
            string[] split = select.Split(',');
            string id = Array.Find(split, s => s.Substring(0, 2).Equals("id"));
            
            return id.Substring(4);
        }
    }
}