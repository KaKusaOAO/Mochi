using System.Collections.Generic;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier;

public delegate IEnumerable<TS> RedirectModifier<TS>(CommandContext<TS> context);