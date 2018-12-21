using System;
using System.Collections.Generic;
using System.Linq;
using clk.Models;
using clk.Resources;
using Random = clk.Resources.Random;

namespace clk.Controllers
{
    public class ArgumentController
    {
        public List<Argument> argList { get; }

        public bool isBoard { get; set; }
        public bool isList { get; set; }
        public bool isCard { get; set; }
        public bool isChecklist { get; set; }
        public bool isNew { get; set; }

        public ArgumentController(string[] args)
        {
            argList = new List<Argument>();

            isBoard = false;
            isList = false;
            isCard = false;
            isChecklist = false;
            isNew = false;
            
            sortArgs(args);
        }

        /// <summary>
        /// This is used to correct the input value to a list index
        /// </summary>
        /// <param name="input">The input value</param>
        /// <returns>input value - 1</returns>
        private string indexValue(int input)
        {
            input--;
            return input.ToString();
        }
        
        /// <summary>
        /// This will sort the init parameters into a global dictionary
        /// of parameter => value
        /// </summary>
        /// <param name="args">The arguments as received from Main(args)</param>
        private void sortArgs(string[] args)
        {
            
            // Group the arguments by key => value(s)
            int br = 0;
            Argument a = new Argument();
            Argument profile = new Argument{key = "--user"};
            foreach (string arg in args)
            {
                if (arg.Equals(""))
                    continue;
                
                // Handle if it is a profile request (login ex)
                if (arg.Equals("--login") || arg.Equals("--new-profile"))
                {
                    Program.user.email = arg;
                    continue;
                }
                if (arg.Equals("--password"))
                {
                    string pw = Random.hashString(arg);
                    Program.user.password = pw;
                    continue;
                }
                
                // If the --new keyword is used, note it
                if (arg.Length >= 6 && arg.Substring(0, 6).Equals("--new-")
                    || arg.Equals("--comment") || arg.Equals("--description"))
                    isNew = true;
                

                // If KEY in argument
                if (arg.Substring(0, 1).Equals("-"))
                {   
                    // Don't add it to our list if empty key (first instance i.e)
                    if (br != 0)
                        argList.Add(a);

                    string key = arg;
                    
                    // Clean the keys a little - 
                    // Becomes a benefit elsewhere in the script,
                    // because we don't have to match as many values.
                    if (arg.Equals("--help"))
                        key = "-h";
                    if (arg.Equals("--about"))
                        key = "-a";
                    if (arg.Equals("--board"))
                        key = "-b";
                    if (arg.Equals("--list"))
                        key = "-l";
                    if (arg.Equals("--card"))
                        key = "-c";
                    if (arg.Equals("--check"))
                        key = "-p";
                    
                    
                    // Notice how deep we are for the "view"
                    if (key.Equals("-b"))
                        isBoard = true;
                    if (key.Equals("-l"))
                        isList = true;
                    if (key.Equals("-c"))
                        isCard = true;
                    if (key.Equals("-p"))
                        isChecklist = true;
                    
                    
                    // Initialize a new argument
                    a = new Argument {key = key};
                    continue;
                }

                
                string value = arg;
                
                // Correct for List indexing
                if (   a.key.Equals("-b")
                    || a.key.Equals("-l")
                    || a.key.Equals("-c")
                    || a.key.Equals("-p")
                    || a.key.Equals("--point")
                    || a.key.Equals("--cloud-save")
                    || a.key.Equals("--cloud-get")
                    || a.key.Equals("--del-board")
                    || a.key.Equals("--del-list")
                    || a.key.Equals("--del-card")
                    || a.key.Equals("--del-check")
                    || a.key.Equals("--del-point")
                    || a.key.Equals("--del-comment")
                    && Validators.isInt(arg))
                    value = indexValue(int.Parse(arg));
                
                // If NOT KEY, but VALUE, add to the Argument value List
                a.value.Add(value);
                br++;
            }
            
            argList.Add(a);
            
            // If both username and password is set,
            // add the profile argument to the list
            if (profile.value.Count == 2)
                argList.Add(profile);
        }
    }
}