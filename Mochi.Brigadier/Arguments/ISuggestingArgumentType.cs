using System.Threading.Tasks;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Suggests;

namespace Mochi.Brigadier.Arguments;

public interface ISuggestingArgumentType : IArgumentType
{
    Task<Suggestions> ListSuggestions<TS>(CommandContext<TS> context, SuggestionsBuilder builder);
}