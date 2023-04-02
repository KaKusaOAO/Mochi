using System.Threading.Tasks;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier;

public interface ICommand<TS>
{
    Task<int> Run(CommandContext<TS> context);
}

public delegate int CommandDelegate<TS>(CommandContext<TS> context);

public delegate Task<int> CommandDelegateAsync<TS>(CommandContext<TS> context);

public delegate Task CommandDelegateNoResult<TS>(CommandContext<TS> context);