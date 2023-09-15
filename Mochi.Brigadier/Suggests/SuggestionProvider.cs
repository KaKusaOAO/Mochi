using System.Threading.Tasks;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier.Suggests;

public delegate Task<Suggestions> SuggestionProvider<T>(CommandContext<T> context, SuggestionsBuilder builder);