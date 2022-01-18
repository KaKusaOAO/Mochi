using KaLib.Brigadier.Context;

namespace KaLib.Brigadier.Suggests;

public delegate Task<Suggestions> SuggestionProvider<TS>(CommandContext<TS> context, SuggestionsBuilder builder);