using System;

namespace clk.Resources
{
    public static class Time
    {

        public static string timestamp(string format="dd.MM.yyyy HH:mm")
        {
            return DateTime.Now.ToString(format);
        }
    }
}