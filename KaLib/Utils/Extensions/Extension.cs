using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaLib.Utils.Extensions
{
    public static class Extension
    {
        public static string JoinStrings(this IEnumerable<string> input, string separator)
        {
            var result = "";
            if (input == null) return result;
            
            result = input.Aggregate(result, (current, entry) => current + separator + entry);
            return result.Length == 0 ? "" : result.Substring(separator.Length);
        }

        public static string Hexdump(this IEnumerable<byte> data) => 
            data.Select(t => $"{t:x2}").JoinStrings(" ").Trim();
    }
}
