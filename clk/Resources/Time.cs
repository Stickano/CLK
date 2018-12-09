using System;

namespace clk.Resources
{
    public static class Time
    {

        public static string timestamp(string format="dd.MM.yyyy hh:mm")
        {
            return DateTime.Now.ToString(format);
        }
    }
}