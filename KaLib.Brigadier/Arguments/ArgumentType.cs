using KaLib.Brigadier.Context;
using KaLib.Brigadier.Suggests;

namespace KaLib.Brigadier.Arguments;

public interface IArgumentType
{
    object Parse(StringReader reader);
    
    Task<Suggestions> ListSuggestions<TS>(CommandContext<TS> context, SuggestionsBuilder builder)
    {
        return Suggestions.Empty();
    }

    IEnumerable<string> GetExamples()
    {
        return Array.Empty<string>();
    }
}

public interface IArgumentType<T> : IArgumentType {
    T Parse(StringReader reader);

    object IArgumentType.Parse(StringReader reader) => Parse(reader);
}
