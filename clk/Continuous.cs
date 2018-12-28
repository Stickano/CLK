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

        
        /// <summary>
        /// This will keep the application running in a constant loop (until user exits of course).
        /// This is horrible put together (so far at least), and just barely working. Best of luck with this piece of garbage. 
        /// </summary>
        public void loopApp()
        {
            // Run this sucker in forever, and ever, and ever, and ever...
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


                // These will later be determined. How many steps a user can go right and down (X and Y positions)
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
                        controls.yPos = yMaxPos - 1; 

                    // Display 3 lists at the time. 
                    // And set the correct index for the first shown list.
                    int startPos = 0;
                    if (controls.xPos - 3 >= 0)
                        startPos = controls.xPos - 2;

                    // Write out the lists, and the cards associated to the current selected list
                    List<Card> cardsToRead = caController.getCards();

                    if (!cardsToRead.Any())
                        controls.yPos = 0;
                    
                    write.writeList(liController.getLists().GetRange(startPos, 3), controls.xPos, controls.yPos, startPos, cardsToRead);
                    //write.writeLists(liController.getLists(), startPos, controls.xPos, controls.yPos, cardsToRead);
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

                
                // Display menu
                write.writeMenu(isBoard, isCard);

                
                // Ask for user input
                Console.WriteLine();
                Console.WriteLine();
                ConsoleKey answer = Console.ReadKey().Key;

                // If the user just navigated (x and y pos in controls)
                // Continue to reload the user interface.
                int response = controls.cursorAction(answer, xMaxPos, yMaxPos);
                if (response == -1)
                    continue;

                // If we selected enter without the board is set,
                // Select a board
                if (!isBoard)
                {
                    // Create a new board
                    if (response == 4)
                    {
                        string newBoard = write.createValue("Board");
                        if (!newBoard.Equals(""))
                            ovController.createBoard(newBoard);

                        boardNum = ovController.getBoards().Count - 1;
                        iniOvController();
                        continue;
                    }
                    
                    // Update board name
                    if (response == 2)
                    {
                        Board b = ovController.getBoards()[controls.yPos];
                        string newName = write.updateValue(b.name);
                        if (!newName.Equals(""))
                            ovController.updateBoard(newName, b.id);
                    }

                    // Delete a board
                    if (response == 3)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Are you sure you wanna delete this board: " + ovController.getBoards()[controls.yPos].name);
                        if (!EyeCandy.confirm())
                            continue;
                        
                        ovController.deleteBoard(ovController.getBoards()[controls.yPos].id);
                        if (controls.yPos != 0)
                            controls.yPos--;
                    }

                    if (response == 1)
                    {
                        boardNum = controls.yPos;
                        iniOvController();
                        continue;
                    }
                }

                // If the user select a card, initialize the card controller
                if (isBoard && !isCard)
                {
                    
                    // Update list name
                    if (response == 2 && controls.yPos == 0)
                    {
                        string newName = write.updateValue(listName);
                        if (!newName.Equals(""))
                            liController.updateList(newName, listId);
                        continue;
                    }
                    
                    // If the user deletes a list
                    if (response == 3 && controls.yPos == 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Are you sure you wanna delete this list: " + listName);
                        if (!EyeCandy.confirm())
                            continue;
                        
                        liController.deleteList(listId);
                        if (controls.xPos != 0)
                            controls.xPos--;

                        listId = null;
                        listName = null;
                        listNum = -1;
                        
                        continue;
                    }

                    // If the user deletes a card.
                    if (response == 3 && controls.yPos != 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Are you sure you wanna delete this card: " + caController.getCards()[controls.yPos-1].name);
                        if (!EyeCandy.confirm())
                            continue;
                        
                        Card c = caController.getCards()[controls.yPos - 1];
                        caController.deleteCard(c.id);
                    }

                    // If the user creates a new list
                    if (response == 4)
                    {
                        string list = write.createValue("List");
                        if (!list.Equals(""))
                            liController.createList(list);

                        controls.xPos = liController.getLists().Count - 1;
                        continue;
                    }

                    // Create a new card
                    if (response == 5)
                    {
                        string card = write.createValue("Card");
                        if (!card.Equals(""))
                            caController.createCard(card);

                        controls.yPos = caController.getCards().Count;
                        continue;
                    }

                    // Select a card
                    if (response == 1 && controls.yPos != 0)
                    {
                        cardNum = controls.yPos - 1; // -1 to correct index (top (0) is list headline)
                        iniCaController(listId);
                        controls.yPos = 1; // Start at description
                    }
                }
                
                
                // Okay then boi, do some magic. When card is selected. 
                // We should get various returns from Controls. Act accordingly here.
                if (isCard)
                {
                    object selectedElement = ObjectValues.getObjectInYPos(caController, cardId, controls.yPos);

                    // Click a checklist point
                    if (response == 1)
                    {
                        if (selectedElement is ChecklistPoint)
                        {
                            ChecklistPoint p = (ChecklistPoint) selectedElement;
                            caController.clickPoint(p.id);
                        }
                    }

                    // Update a description on a card
                    if (response == 2 && controls.yPos == 1)
                        caController.createDescription(write.createValue("Description"), cardId);

                    // Update the name of a card
                    if (response == 2 && controls.yPos == 0)
                    {
                        string newName = write.updateValue(cardName);
                        if (!newName.Equals(""))
                            caController.updateCard(newName, cardId);
                    }
                    
                    // If user is updating an element
                    if (response == 2)
                        updateElement(selectedElement);
                    
                    // If user is deleting an element
                    if (response == 3)
                        deleteElement(selectedElement);
                    
                    // If the user is creating a new checklist
                    if (response == 4)
                    {
                        string ck = write.createValue("Checklist");
                        if (!ck.Equals(""))
                            caController.createChecklist(ck, cardId);
                    }

                    // If the user is creating a new checklist point
                    if (response == 5)
                    {
                        string p = write.createValue("Point");
                        if (!p.Equals(""))
                            caController.createChecklistPoint(p, checkId);
                    }
                }
            }
        }

        /// <summary>
        /// If the user pressed d at an element, this will run.
        /// </summary>
        /// <param name="element">The element at the cursor position</param>
        private void deleteElement(object element)
        {

            Console.WriteLine();
            
            if (element is Checklist)
            {
                Checklist c = (Checklist) element;
                Console.WriteLine("Are you sure you wanna delete checklist: " + c.name);
                if (EyeCandy.confirm())
                    caController.deleteChecklist(c.id);
            }

            if (element is ChecklistPoint)
            {
                ChecklistPoint point = (ChecklistPoint) element;
                Console.WriteLine("Are you sure you wanna delete point: "+point.name);
                if (EyeCandy.confirm())
                    caController.deletePoint(point.id);
            }

            if (element is Comment)
            {
                Comment comment = (Comment) element;
                Console.WriteLine("Are you sure you wanna delete comment created on: " + comment.created);
                if (EyeCandy.confirm())
                    caController.deleteComment(comment.id);
            }  
        }

        /// <summary>
        /// Updates the element at the cursor
        /// </summary>
        /// <param name="element">The element to update (at the cursor position)</param>
        private void updateElement(object element)
        {
            if (element is Checklist)
            {
                Checklist c = (Checklist) element;
                string update = write.updateValue(c.name);
                if (update.Equals(""))
                    return;
                
                caController.updateChecklist(update, c.id);
            }

            if (element is ChecklistPoint)
            {
                ChecklistPoint point = (ChecklistPoint) element;
                string update = write.updateValue(point.name);
                if (update.Equals(""))
                    return;
                
                caController.updateChecklistPoint(update, point.id);
            }

            if (element is Comment)
            {
                Comment comment = (Comment) element;
                string update = write.updateValue(comment.comment);
                if (update.Equals(""))
                    return;
                
                caController.updateComment(update, comment.id);
            }   
        }
        
    }
}