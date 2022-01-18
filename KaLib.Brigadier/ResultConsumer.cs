using KaLib.Brigadier.Context;

namespace KaLib.Brigadier;

public delegate void ResultConsumer<TS>(CommandContext<TS> context, bool success, int result);