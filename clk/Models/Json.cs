
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using clk.Controllers;
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
        /// <param name="data">A generic list of content to be written</param>
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
            if (!File.Exists("Json/settings.json"))
                File.Create("Json/settings.json").Close();
        }

        /// <summary>
        /// A method to store the user profile on the disc.
        /// Used for auto-login to the cloud.
        /// </summary>
        /// <param name="profile">The profile to store.</param>
        public static void storeProfileDetails(Profile profile)
        {
            // Make sure we have uname, pw and id.
            if (profile.id == null || profile.email == null || profile.password == null)
                return;
            
            // Write the profile to the Json file.
            writeFile(profile, "profile.json");
        }

        /// <summary>
        /// Another method to write a Json file,
        /// this one accepts a single object, instead of a list of objects.
        /// This is used to write the settings json i.e.
        /// </summary>
        /// <param name="obj">The object to write</param>
        /// <param name="file">Which file to write to</param>
        /// <typeparam name="T">Any object. SettingsController is used i.e.</typeparam>
        public static void writeFile<T>(T obj, string file)
        {
            string jsonDir = "Json/";
            var path = Path.Combine(jsonDir + file);
            
            // Create file, if it doesn't already exists
            if (!File.Exists(path))
                File.Create(path).Close();
            
            using (StreamWriter writer = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
            }
        }
    }
}