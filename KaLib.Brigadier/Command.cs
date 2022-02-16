using KaLib.Brigadier.Context;

namespace KaLib.Brigadier;

public interface ICommand<TS>
{
    const int SingleSuccess = 1;

    Task<int> Run(CommandContext<TS> context);
}

public delegate int CommandDelegate<TS>(CommandContext<TS> context);

public delegate Task<int> CommandDelegateAsync<TS>(CommandContext<TS> context);

public delegate Task CommandDelegateResultless<TS>(CommandContext<TS> context);