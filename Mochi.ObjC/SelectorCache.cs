using System.Collections.Concurrent;

namespace Mochi.ObjC;

internal static class SelectorCache
{
    private static readonly SemaphoreSlim _lock = new(1);
    private static readonly ConcurrentDictionary<string, Selector> _cache = new();

    public static Selector Get(string name)
    {
        _lock.Wait();
        try
        {
            if (_cache.TryGetValue(name, out var result))
                return result;

            var selector = new Selector(name);
            _ = _cache.TryAdd(name, selector);
            return selector;
        }
        finally
        {
            _lock.Release();
        }
    }

    public static void StoreCache(string name, Selector selector)
    {
        var selName = selector.Name;
        if (selName != name)
            throw new Exception($"Selector name mismatch! {name} != {selName}");
        
        _lock.Wait();
        
        try
        {
            _ = _cache.TryAdd(name, selector);
        }
        finally
        {
            _lock.Release();
        }
    }
}