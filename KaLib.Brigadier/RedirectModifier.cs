using KaLib.Brigadier.Context;

namespace KaLib.Brigadier;

public delegate IEnumerable<TS> RedirectModifier<TS>(CommandContext<TS> context);