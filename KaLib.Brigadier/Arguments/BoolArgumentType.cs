using System.Collections.Generic;
using System.Threading.Tasks;
using KaLib.Brigadier.Context;
using KaLib.Brigadier.Suggests;

namespace KaLib.Brigadier.Arguments
{
    public class BoolArgumentType : IArgumentType<bool>
    {
        private static IEnumerable<string> EXAMPLES = new List<string> { "true", "false" };

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

#if NET6_0_OR_GREATER
        public bool Parse(StringReader reader)
#else
        public object Parse(StringReader reader)
#endif
        {
            return reader.ReadBoolean();
        }

        public Task<Suggestions> ListSuggestions<S>(CommandContext<S> context, SuggestionsBuilder builder)
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
            return EXAMPLES;
        }
    }
}