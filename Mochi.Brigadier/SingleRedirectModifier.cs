using Mochi.Brigadier.Context;

namespace Mochi.Brigadier;

public delegate T SingleRedirectModifier<T>(CommandContext<T> context);