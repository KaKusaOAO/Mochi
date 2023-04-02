using Mochi.Brigadier.Context;

namespace Mochi.Brigadier;

public delegate void ResultConsumer<TS>(CommandContext<TS> context, bool success, int result);