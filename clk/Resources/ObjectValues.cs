using System;
using System.Collections.Generic;
using clk.Controllers;
using clk.Models;

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

        /// <summary>
        /// This will find which element the user is positioned at on the Y position in the "view".
        /// It is used when displaying a card. It will loop through all the elements displayed for that card,
        /// in the view, and return the object where the cursor was positioned. 
        /// </summary>
        /// <param name="caController">The cardcontroller which should be initialised by this stage</param>
        /// <param name="cardId">The ID of the card we are working in</param>
        /// <param name="yPos">The cursor position</param>
        /// <returns>Object of either Checklist, ChecklistPoint or Comment</returns>
        public static object getObjectInYPos(CardController caController, string cardId, int yPos)
        {
            int br = 2; // First two are name and description (in the view)

            // First loop through the checklists
            foreach (Checklist checklist in caController.getChecklists(cardId))
            {
                Program.checkId = checklist.id;
                Program.checkNum = br - 2;
                Program.checkName = checklist.name;
                
                if (br == yPos)
                    return checklist;

                br++;

                // Loop through the points in the checklist
                foreach (ChecklistPoint point in caController.getChecklistPoints(checklist.id))
                {
                    if (br == yPos)
                        return point;

                    br++;
                }   
            }
            
            // Then loop through the comments 
            foreach (Comment comment in caController.getComments(cardId))
            {
                if (br == yPos)
                    return comment;

                br++;
            }

            return null;
        } 
    }
}