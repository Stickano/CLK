using System;

namespace clk.Resources
{
    public static class About
    {
        
        /// <summary>
        /// This is the CLI interaction with this application.
        /// When this messages has been shown, the application will
        /// exit and no further actions will be performed.
        /// </summary>
        public static void Usage()
        {
            Console.WriteLine("CLI Kanban - usage options and examples:");
            Console.WriteLine();
            Console.WriteLine("-h --help    This message");
            Console.WriteLine("-a --about   Version and program information");
            Console.WriteLine();
            Console.WriteLine("-b --board   Select board");
            Console.WriteLine("-l --list    Select list");
            Console.WriteLine("-c --card    Select card");
            Console.WriteLine();
            Console.WriteLine("--new-board    Create a new board");
            Console.WriteLine("--new-list     Create a new list");
            Console.WriteLine("--new-card     Create a new card");
            Console.WriteLine("--description  Description is available for Cards only!");
            Console.WriteLine("--comment      Comment is available for Cards only!");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Usage examples:");
            Console.WriteLine("Get an overview of all boards:     clk -b");
            Console.WriteLine("Choose a board and see its lists:  clk -b=2");
            Console.WriteLine("Choose a list and see its cards:   clk -b=2 -l=1");
            Console.WriteLine("Choose a card and see its content: clk -b=2 -l=1 -c=7");
            Console.WriteLine();
            Console.WriteLine("Create a new board:    clk --new-board=\"My Project\"");
            Console.WriteLine("Create a new list:     clk -b=2 --new-list=\"TODO\"");
            Console.WriteLine("Create a new card:     clk -b=2 -l=1 --new-card=\"Profit\"");
            Console.WriteLine();
            Console.WriteLine("New board w/ lists:    clk --new-board=\"My Project\" --new-list=\"TODO\" --new-list=\"Done\"");
            Console.WriteLine("New cards:             clk -b=2 -l=1 --new-card=\"This\" --new-card=\"That\"");
            Console.WriteLine();
            Console.WriteLine("Add description to a card:   clk -b=2 -l=1 -c=5 --description=\"This is information about that thing..\"");
            Console.WriteLine("Add comment to a card:       clk -b=2 -l=1 -c=5 --comment=\"When this, remember that..\"");
            
            Environment.Exit(0);
        }
    }
}