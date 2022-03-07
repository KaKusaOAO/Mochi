using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KaLib.Brigadier.Context;
using KaLib.Brigadier.Suggests;

namespace KaLib.Brigadier.Arguments
{
    public interface IArgumentType
    {
        object Parse(StringReader reader);
    
        Task<Suggestions> ListSuggestions<TS>(CommandContext<TS> context, SuggestionsBuilder builder)
#if !NET6_0_OR_GREATER
        ;
#else
        {
            return Suggestions.Empty();
        }
#endif
        IEnumerable<string> GetExamples()
#if !NET6_0_OR_GREATER
        ;
#else
        {
            return Array.Empty<string>();
        }
#endif
    }

    public interface IArgumentType<T> : IArgumentType {
#if NET6_0_OR_GREATER
        new T Parse(StringReader reader);

        object IArgumentType.Parse(StringReader reader)
        {
            return Parse(reader);
        }
#endif
    }
}
