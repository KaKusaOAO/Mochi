using System.Collections;

namespace Mochi.StreamKit.Twitch.API;

public abstract class SimpleEnumCollection<T> : ICollection<T>
{
    private readonly HashSet<T> _modes = new();

    public IEnumerator<T> GetEnumerator() => _modes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item) => _modes.Add(item);

    public void Clear() => _modes.Clear();

    public bool Contains(T item) => _modes.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => _modes.CopyTo(array, arrayIndex);

    public bool Remove(T item) => _modes.Remove(item);

    public int Count => _modes.Count;

    public bool IsReadOnly => false;

    public abstract T CreateFromString(string value);

    public abstract string CreateFromValue(T value);
}