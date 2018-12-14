using System;
using System.Security.Cryptography;
using System.Text;

namespace clk.Resources
{
    public static class Random
    {
        /// <summary>
        /// A method that will return an unique ID
        /// ex: 0f8fad5b-d9cb-469f-a165-70867728950e
        /// </summary>
        /// <returns>String uniqueId</returns>
        public static string guid()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// A function that will hash a string with SHA256 
        /// </summary>
        /// <param name="value">The value to hash</param>
        /// <returns>The hashed value</returns>
        public static string hashString(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}