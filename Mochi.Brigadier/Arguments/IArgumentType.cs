using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Suggests;

namespace Mochi.Brigadier.Arguments;

public interface IArgumentType
{
    object Parse(StringReader reader);
    
    public Task<Suggestions> ListSuggestionsAsync<TSource>(CommandContext<TSource> context, SuggestionsBuilder builder) => 
        Suggestions.Empty();

    public ICollection<string> Examples => new List<string>();
}

public interface IArgumentType<out T> : IArgumentType
{
    new T Parse(StringReader reader);
    object IArgumentType.Parse(StringReader reader) => Parse(reader)!;
}