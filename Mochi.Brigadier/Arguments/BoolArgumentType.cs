using System.Collections.Generic;
using System.Linq;
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

    public static BoolArgumentType Bool() => new();

    public static bool GetBool<T>(CommandContext<T> context, string name) => context.GetArgument<bool>(name);

    public bool Parse(StringReader reader) => reader.ReadBoolean();

    public Task<Suggestions> ListSuggestionsAsync<T>(CommandContext<T> context, SuggestionsBuilder builder)
    {
        if ("true".StartsWith(builder.RemainingLowerCase))
        {
            builder.Suggest("true");
        }

        if ("false".StartsWith(builder.RemainingLowerCase))
        {
            builder.Suggest("false");
        }

        return builder.BuildAsync();
    }

    public ICollection<string> Examples => _examples.ToList();
}