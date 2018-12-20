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
        public static void usage()
        { // TODO: Should have a --help [category] kinda' approach.
            Ascii.clk();
            Console.WriteLine("This tool is very versatile and the parameters can be combined in a number of ways.");
            Console.WriteLine("Below information is just to get you started. For more information and usage examples, visit: www.website.com");
            Console.WriteLine();
            Console.WriteLine("-h --help      This message");
            Console.WriteLine("-a --about     Version and program information");
            Console.WriteLine();
            Console.WriteLine("-b --board     Select board");
            Console.WriteLine("-l --list      Select list");
            Console.WriteLine("-c --card      Select card");
            //Console.WriteLine("-p --check     Select (Points) checklist");
            Console.WriteLine();
            Console.WriteLine("--new-board    Create a new board");
            Console.WriteLine("--new-list     Create a new list");
            Console.WriteLine("--new-card     Create a new card");
            Console.WriteLine("--new-check    Create a new checklist");
            Console.WriteLine("--new-point    Create a new checkpoint, in a checklist");
            Console.WriteLine();
            Console.WriteLine("--description  Description is available for Cards only! (1 per card)");
            Console.WriteLine("--comment      Comment is available for Cards only! (Many per card)");
            Console.WriteLine("--point        Check/uncheck a point in a checklist");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Usage examples:");
            Console.WriteLine("Boards overview:       clk -b");
            Console.WriteLine("Get lists in a board:  clk -b 2");
            Console.WriteLine("Get cards in a list:   clk -b 2 -l 1");
            Console.WriteLine("Get card content:      clk -b 2 -l 1 -c 7");
            Console.WriteLine();
            Console.WriteLine("Create a new board:    clk --new-board \"My Project\"");
            Console.WriteLine("Create a new list:     clk -b 2 --new-list TODO");
            Console.WriteLine("Create a new card:     clk -b 2 -l 1 --new-card Profit");
            Console.WriteLine();
            Console.WriteLine("New board w/ lists:    clk --new-board \"My Project\" --new-list TODO Doing Done");
            Console.WriteLine("New cards:             clk -b 2 -l 1 --new-card This That");
            Console.WriteLine();
            Console.WriteLine("Create a checklist:            clk -b 2 -l 1 -c 5 --new-check \"New Checklist\"");
            Console.WriteLine("Create a checklist (-p) point: clk -b 2 -l 1 -c 5 -p 3 --new-point \"New checklist point\"");
            Console.WriteLine("Create a checklist w/ points:  clk -b 2 -l 1 -c 5 --new-check \"To Profit\" --new-point This That");
            Console.WriteLine();
            Console.WriteLine("Add description to a card:     clk -b 2 -l 1 -c 5 --description \"This is information about that thing..\"");
            Console.WriteLine("Add comment to a card:         clk -b 2 -l 1 -c 5 --comment \"When this, remember that..\"");
            
            Environment.Exit(0);
        }
    }
}