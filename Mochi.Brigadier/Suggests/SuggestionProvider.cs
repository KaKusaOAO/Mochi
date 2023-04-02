using System.Threading.Tasks;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier.Suggests;

public delegate Task<Suggestions> SuggestionProvider<TS>(CommandContext<TS> context, SuggestionsBuilder builder);