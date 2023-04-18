using System;
using System.Collections.Generic;

namespace Mochi.JS;

public class HandlePool<T>
{
    private Dictionary<object, T> _pool = new();
    
    public T Get(object handle)
    {
        if (_pool.TryGetValue(handle, out var value))
        {
            return value;
        }

#pragma warning disable IL2087
        return (T)Activator.CreateInstance(typeof(T), handle);
#pragma warning restore IL2087
    }

    private static HandlePool<T> _shared;
    public static HandlePool<T> Shared => _shared ??= new();
    
    public void Release(object handle)
    {
        _pool.Remove(handle);
    }
}