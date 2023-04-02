using System.Collections.Generic;
using System.Threading.Tasks;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Suggests;

namespace Mochi.Brigadier.Arguments;

public class BoolArgumentType : IArgumentType<bool>
{
    private static IEnumerable<string> _examples = new List<string> { "true", "false" };

    private BoolArgumentType()
    {
    }

    public static BoolArgumentType Bool()
    {
        return new BoolArgumentType();
    }

    public static bool GetBool<T>(CommandContext<T> context, string name)
    {
        return context.GetArgument<bool>(name);
    }

    public bool Parse(StringReader reader) => reader.ReadBoolean();

    public Task<Suggestions> ListSuggestions<TS>(CommandContext<TS> context, SuggestionsBuilder builder)
    {
        if ("true".StartsWith(builder.GetRemainingLowerCase()))
        {
            builder.Suggest("true");
        }

        if ("false".StartsWith(builder.GetRemainingLowerCase()))
        {
            builder.Suggest("false");
        }

        return builder.BuildFuture();
    }

    public IEnumerable<string> GetExamples()
    {
        return _examples;
    }
}