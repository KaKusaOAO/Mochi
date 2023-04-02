#if NETCOREAPP3_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
#define HAS_CODEANALYSIS
#define HAS_ASYNC_ENUMERATOR
#endif

#if HAS_ASYNC_ENUMERATOR
#endif
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Mochi.Utils.Extensions;

public static class Extension
{
#if HAS_CODEANALYSIS
    [return: NotNull]
#endif
    public static string JoinStrings(
#if HAS_CODEANALYSIS
        [AllowNull]
#endif
        this IEnumerable<string> input, string separator)
    {
        var result = "";
        if (input == null) return result;
            
        result = input.Aggregate(result, (current, entry) => current + separator + entry);
        return result.Length == 0 ? "" : result.Substring(separator.Length);
    }

    public static string Hexdump(this IEnumerable<byte> data) => 
        data.Select(t => $"{t:x2}").JoinStrings(" ").Trim();


#if HAS_ASYNC_ENUMERATOR
    public static async Task<IEnumerable<T>> CollectAllAsync<T>(this IAsyncEnumerable<T> enumerator)
    {
        var result = new List<T>();
        await foreach (var item in enumerator)
        {
            result.Add(item);
        }

        return result;
    }
#endif
}