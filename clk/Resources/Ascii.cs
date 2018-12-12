using System;

namespace clk.Resources
{
    public static class Ascii
    {
        /// <summary>
        /// Prints out CLK. in ascii characters
        /// </summary>
        public static void clk()
        {
            Console.WriteLine("  ___ _    _  __");
            Console.WriteLine(" / __| |  | |/ /");
            Console.WriteLine("| (__| |__| ' < _ ");
            Console.WriteLine(" \\___|____|_|\\_(_)");
            Console.WriteLine("");
        }

        /// <summary>
        /// Prints out CLK.Board in ascii characters
        /// </summary>
        public static void clkBoard()
        {
            Console.WriteLine("  ___ _    _  __  ___                   _ ");
            Console.WriteLine(" / __| |  | |/ / | _ ) ___  __ _ _ _ __| |");
            Console.WriteLine("| (__| |__| ' < _| _ \\/ _ \\/ _` | '_/ _` |");
            Console.WriteLine(" \\___|____|_|\\_(_)___/\\___/\\__,_|_| \\__,_|");
            Console.WriteLine("");
        }

        /// <summary>
        /// Prints out CLK.Card in ascii characters
        /// </summary>
        public static void clkCard()
        {
            Console.WriteLine("  ___ _    _  __  ___             _ ");
            Console.WriteLine(" / __| |  | |/ / / __|__ _ _ _ __| |");
            Console.WriteLine("| (__| |__| ' < | (__/ _` | '_/ _` |");
            Console.WriteLine(" \\___|____|_|\\_(_)___\\__,_|_| \\__,_|");
            Console.WriteLine("");
        }

        /// <summary>
        /// Print out CLK.List in ascii characters
        /// </summary>
        public static void clkList()
        {
            Console.WriteLine("  ___ _    _  __  _    _    _   ");
            Console.WriteLine(" / __| |  | |/ / | |  (_)__| |_ ");
            Console.WriteLine("| (__| |__| ' < _| |__| (_-<  _|");
            Console.WriteLine(" \\___|____|_|\\_(_)____|_/__/\\__|");
            Console.WriteLine("");
        }
        
        /// <summary>
        /// This is a little easter egg
        /// </summary>
        public static void clkEaster()
        {
            Console.WriteLine(" ____        __          __  __     ");
            Console.WriteLine("/\\  _`\\     /\\ \\        /\\ \\/\\ \\");
            Console.WriteLine("\\ \\ \\/\\_\\   \\ \\ \\       \\ \\ \\/'/'");
            Console.WriteLine(" \\ \\ \\/_/_   \\ \\ \\  __   \\ \\ , <");
            Console.WriteLine("  \\ \\ \\L\\ \\   \\ \\ \\L\\ \\   \\ \\ \\\\`\\");
            Console.WriteLine("   \\ \\____/    \\ \\____/    \\ \\_\\ \\_\\");
            Console.WriteLine("    \\/___/      \\/___/      \\/_/\\/_/");
            Console.WriteLine();
        }
    }
}