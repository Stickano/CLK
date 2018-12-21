using System.Linq;
using System.Reflection;

namespace clk.Models
{
    public class Label
    {
        private string[] labelsAval = {"yellow", "green", "blue", "red", "cyan"};

        public string label { get; set; }

        /// <summary>
        /// Check if a color is available (used to match user-input).
        /// </summary>
        /// <param name="color">The selected color name</param>
        /// <returns>True/False if available or not</returns>
        public bool isAvail(string color)
        {
            if (labelsAval.Contains(color))
                return true;
            return false;
        }
    }
}