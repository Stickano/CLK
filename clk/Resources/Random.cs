using System;

namespace clk.Resources
{
    public static class Random
    {
        /// <summary>
        /// A method that will return an unique ID
        /// ex: 0f8fad5b-d9cb-469f-a165-70867728950e
        /// </summary>
        /// <returns>String uniqueId</returns>
        public static Guid guid()
        {
            Guid g = Guid.NewGuid();
            return g;
        }
    }
}