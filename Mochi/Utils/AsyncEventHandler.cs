using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mochi.Utils;

public class AsyncEventHandler<T>
{
    private readonly List<T> _handlers = new();
    
    public void AddHandler(T handler)
    {
        _handlers.Add(handler);
    }

    public void RemoveHandler(T handler)
    {
        _handlers.Remove(handler);
    }

    public async Task InvokeAsync(Func<T, Task> invoke)
    {
        var tasks = _handlers.Select(invoke);
        await Task.WhenAll(tasks);
    }

    public IEnumerable<T> Handlers => _handlers;
}