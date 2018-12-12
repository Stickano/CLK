
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;

namespace clk.Models
{
    public class Json
    {
        private string jsonDir = "Json/";
        private string file;

        public Json(string file)
        {
            this.file = file;
        }

        /// <summary>
        /// This will write a List of T into one of
        /// the available Json files.
        /// </summary>
        /// <param name="data"></param>
        public void writeFile<T>(List<T> data)
        {
            using (StreamWriter writer = File.CreateText(jsonDir + file))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, data);
            }
        }

        /// <summary>
        /// This will read any of the available Json files
        /// and return the content as a List of T
        /// You will have to typecast the object into the
        /// correct model.
        /// </summary>
        /// <returns>List of the content from the Json file</returns>
        public List<T> readFile<T>()
        {
            List<T> result = new List<T>();
            using (StreamReader read = new StreamReader(jsonDir + file))
            {
                string json = read.ReadToEnd();
                if (new FileInfo(jsonDir + file).Length != 0)
                    result = JsonConvert.DeserializeObject<List<T>>(json);
            }

            return result;
        }

        /// <summary>
        /// This is a static method that is run every time
        /// the application is run. This will make sure,
        /// that the necessary Json files is available. 
        /// </summary>
        public static void isFiles()
        {
            // Create directory.
            // Does nothing if dir already exists.
            Directory.CreateDirectory("Json");

            // Create the Json files, if their are missing.
            // First time use usually.
            if (!File.Exists("Json/boards.json"))
                File.Create("Json/boards.json").Close();
            if (!File.Exists("Json/cards.json"))
                File.Create("Json/cards.json").Close();
            if (!File.Exists("Json/lists.json"))
                File.Create("Json/lists.json").Close();
            if (!File.Exists("Json/comments.json"))
                File.Create("Json/comments.json").Close();
            if (!File.Exists("Json/checklists.json"))
                File.Create("Json/checklists.json").Close();
            if (!File.Exists("Json/points.json"))
                File.Create("Json/points.json").Close();
        }
    }
}