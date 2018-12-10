namespace clk.Resources
{
    public static class Validators
    {
        
        /// <summary>
        /// This method will take a string value,
        /// and check if it is in fact a numeric value.
        /// </summary>
        /// <param name="value">The string value to check</param>
        /// <returns>True/False</returns>
        public static bool isInt(string value)
        {
            int control;
            if (int.TryParse(value, out control))
                return true;
            return false;
        }
    }
}