using System.Threading.Tasks;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Suggests;

namespace Mochi.Brigadier.Arguments;

public interface ISuggestingArgumentType : IArgumentType
{
    Task<Suggestions> ListSuggestionsAsync<TS>(CommandContext<TS> context, SuggestionsBuilder builder);
}