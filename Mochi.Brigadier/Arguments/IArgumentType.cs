using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Suggests;

namespace Mochi.Brigadier.Arguments;

public interface IArgumentType
{
    object Parse(StringReader reader);
}

public interface IArgumentType<out T> : IArgumentType
{
    new T Parse(StringReader reader);
    object IArgumentType.Parse(StringReader reader) => Parse(reader);
}

public static class ArgumentTypeExtensions
{
    public static Task<Suggestions> ListSuggestions<TS>(this IArgumentType type, CommandContext<TS> context,
        SuggestionsBuilder builder)
    {
        if (type is ISuggestingArgumentType suggestingType)
            return suggestingType.ListSuggestions(context, builder);

        return Suggestions.Empty();
    }
    
    public static IEnumerable<string> GetExamples(this IArgumentType type)
    {
        if (type is IArgumentTypeWithExamples typeWithExamples)
            return typeWithExamples.GetExamples();

        return Array.Empty<string>();
    }
}