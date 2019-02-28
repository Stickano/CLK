using System;
using System.IO;
using System.Linq;
using clk.Models;

namespace clk.Controllers
{
    public class SettingsController
    {
        private Json json;
        private Settings settings;
        
        public SettingsController()
        {
            Json.isFiles();
            json = new Json("settings.json");
            if (new FileInfo("Json/settings.json").Length == 0)
                defineFile();

            settings = json.readFileToObject<Settings>();
        }

        /// <summary>
        /// If the settings json file hasn't been defined before (first run i.e),
        /// this will make sure the values are available from that file, by writting it. 
        /// </summary>
        private void defineFile()
        {
            Settings s = new Settings();
            s.defaultBoard = "";
            s.autoSaveToCloud = false;
            s.email = "";
            s.password = "";

            json.writeFile(s);
        }

        /// <summary>
        /// This will confirm, that auto login is possible. 
        /// </summary>
        /// <returns>True/False if credencials is available.</returns>
        public bool autoLoginPossible()
        {
            if (!settings.email.Equals("") && !settings.password.Equals(""))
                return true;
            return false;
        }

        /// <summary>
        /// This will check if the user wan't to auto push changes to the database.
        /// It requires credencials to be stored for auto login.
        /// </summary>
        /// <returns>True/False if the user selected to auto push to db.</returns>
        public bool autoPushToCloud()
        {
            if (autoLoginPossible() && settings.autoSaveToCloud)
                return true;
            return false;
        }

        /// <summary>
        /// Return the selected default board.
        /// </summary>
        /// <returns>String boardId or empty value</returns>
        public string defaultBoard()
        {
            return settings.defaultBoard;
        }

        /// <summary>
        /// Will return the stored credencials.
        /// </summary>
        /// <param name="getPasswordHash">Set true to return the password value instead.</param>
        /// <returns>String Gets the email address.</returns>
        public string getCredencials(bool getPasswordHash = false)
        {
            if (getPasswordHash)
                return settings.password;
            return settings.email;
        }

        /// <summary>
        /// This set the default board ID for the settings.
        /// </summary>
        /// <param name="boardId"></param>
        public void updateDefaultBoard(string boardId = "")
        {
            settings.defaultBoard = boardId;
            json.writeFile(settings);
        }

        /// <summary>
        /// This will remove the stored credencials.
        /// </summary>
        public void unsetCredencials()
        {
            settings.email = "";
            settings.password = "";
            settings.autoSaveToCloud = false;
            json.writeFile(settings);
        }

        /// <summary>
        /// This will store credencials to the settings.json file.
        /// </summary>
        /// <param name="email">The email to save</param>
        /// <param name="hashedPassword">The password to save (in a hashed state)</param>
        public void setCredencials(string email, string hashedPassword)
        {
            settings.email = email;
            settings.password = hashedPassword;
            json.writeFile(settings);
        }

        /// <summary>
        /// This will change the auto update to db (cloud) value from true to false, 
        /// or vice versa.
        /// </summary>
        public void changeAutoUpdateToCloud()
        {
            settings.autoSaveToCloud = !settings.autoSaveToCloud;
            json.writeFile(settings);
        }
        
    }
}

