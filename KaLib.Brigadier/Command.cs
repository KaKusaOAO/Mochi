using KaLib.Brigadier.Context;

namespace KaLib.Brigadier;

public interface ICommand<TS>
{
    const int SingleSuccess = 1;

    int Run(CommandContext<TS> context);
}

public delegate int CommandDelegate<TS>(CommandContext<TS> context);