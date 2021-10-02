using System;
using System.Collections.Generic;
using System.Text;

namespace KaLib.Utils.Extensions
{
    public static class Extension
    {
        public static List<O> Map<I, O>(this IEnumerable<I> input, Func<I, O> mapper)
        {
            List<O> result = new List<O>();
            if (input == null) return result;
            foreach (I i in input)
            {
                result.Add(mapper(i));
            }
            return result;
        }
        
        public static string JoinToString(this IEnumerable<string> input, string separator)
        {
            var result = "";
            if (input == null) return result;
            foreach (string i in input)
            {
                result += separator + i;
            }
            return result.Length == 0 ? "" : result[separator.Length..];
        }

        public static T Find<T>(this IEnumerable<T> input, Func<T, bool> finder)
        {
            if (input == null) return default;
            foreach (var item in input)
            {
                if (finder(item)) return item;
            }

            return default;
        }

        public static string Hexdump(this byte[] data)
        {
            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append($"{t:x2} ");
            }

            return sb.ToString().Trim();
        }
    }
}
