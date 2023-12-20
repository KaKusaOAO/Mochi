using System.Collections.Generic;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier;

public delegate IEnumerable<T> RedirectModifier<T>(CommandContext<T> context);