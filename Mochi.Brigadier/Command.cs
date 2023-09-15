using System.Threading.Tasks;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier;

public interface ICommand<T>
{
    Task<int> Run(CommandContext<T> context);
}

public delegate int CommandDelegate<T>(CommandContext<T> context);

public delegate Task<int> CommandDelegateAsync<T>(CommandContext<T> context);

public delegate Task CommandDelegateNoResult<T>(CommandContext<T> context);