using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LogManager.Helpers
{
    public static class StringHelpers
    {
        public static string MaxLength(this string baseString, int maxLength)
        {
            var result = (string)baseString.Clone();
            if(baseString.Length > maxLength)
            {
                result = result.Substring(0, maxLength) + "...";
            }
            return result;
        }

        public static bool Contains(this string source, string toCheck, StringComparison compRules)
        {
            return source.IndexOf(toCheck, compRules) >= 0;
        }
    }
}