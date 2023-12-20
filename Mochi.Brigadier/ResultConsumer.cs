using Mochi.Brigadier.Context;

namespace Mochi.Brigadier;

public delegate void ResultConsumer<T>(CommandContext<T> context, bool success, int result);