using System.Collections.Concurrent;

namespace Mochi.Utils;

// Directly calling untyped method is not safe.
// Don't do that unless you are sure that the object is of the correct type.
public interface IObjectPool
{
    public object Rent();
    public void Return(object obj);
}

public interface IObjectPool<T> : IObjectPool
{
    public new T Rent();
    object IObjectPool.Rent() => Rent();
    
    public void Return(T obj);
    void IObjectPool.Return(object obj) => Return((T) obj);
}

public class ObjectPool<T> : IObjectPool<T> where T : new()
{
    private static ObjectPool<T> _instance;
    public static ObjectPool<T> Shared => _instance ??= new ObjectPool<T>();

    private readonly ConcurrentBag<T> _bag = new();
    
    public T Rent()
    {
        if (!_bag.TryTake(out var obj)) return new T();
        if (obj is IPooledObject p) p.OnRent();
        return obj;
    }
    
    public void Return(T obj)
    {
        if (obj is IPooledObject p) p.OnReturn();
        _bag.Add(obj);
    }
}

public interface IPooledObject
{
    /// <summary>
    /// Called when the object is rented from the pool.
    /// Note that this is not called when the object is first created.
    /// </summary>
    public void OnRent();
    
    /// <summary>
    /// Called when the object is returned to the pool.
    /// </summary>
    public void OnReturn();
}