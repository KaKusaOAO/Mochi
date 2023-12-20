using System.Collections;

namespace Mochi.Irc;

public class TagCollection : IDictionary<string, IIrcTagValue>
{
    private readonly List<Tag> _tags = new();
    private SemaphoreSlim _lock = new(1, 1);
    
    public class Tag
    {
        public string Key { get; set; }
        public IIrcTagValue Value { get; set; }
    }

    public IEnumerator<KeyValuePair<string, IIrcTagValue>> GetEnumerator() => 
        _tags.Select(t => new KeyValuePair<string, IIrcTagValue>(t.Key, t.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(KeyValuePair<string, IIrcTagValue> item)
    {
        if (_tags.Any(t => t.Key == item.Key)) return;
        _tags.Add(new Tag
        {
            Key = item.Key,
            Value = item.Value
        });
    }

    public void Clear() => _tags.Clear();

    public bool Contains(KeyValuePair<string, IIrcTagValue> item) => _tags.Any(t => t.Key == item.Key && t.Value == item.Value);

    public void CopyTo(KeyValuePair<string, IIrcTagValue>[] array, int arrayIndex)
    {
        var arr = _tags.Select(t => new KeyValuePair<string, IIrcTagValue>(t.Key, t.Value)).ToArray();
        Array.Copy(arr, 0, array, arrayIndex, arr.Length);
    }

    public bool Remove(KeyValuePair<string, IIrcTagValue> item)
    {
        var tag = _tags.FirstOrDefault(t => t.Key == item.Key && t.Value == item.Value);
        return tag != null && _tags.Remove(tag);
    }

    public int Count => _tags.Count;
    public bool IsReadOnly => false;
    
    public void Add(string key, IIrcTagValue value)
    {
        if (_tags.Any(t => t.Key == key)) return;
        _tags.Add(new Tag
        {
            Key = key,
            Value = value
        });
    }

    public bool ContainsKey(string key) => _tags.Any(t => t.Key == key);

    public bool Remove(string key) => _tags.RemoveAll(t => t.Key == key) > 0;

    public bool TryGetValue(string key, out IIrcTagValue value)
    {
        var tag = _tags.FirstOrDefault(t => t.Key == key);
        if (tag == null)
        {
            value = null!;
            return false;
        }

        value = tag.Value;
        return true;
    }

    public IIrcTagValue? this[string key]
    {
        get => _tags.FirstOrDefault(t => t.Key == key)?.Value!;
        set
        {
            var tag = _tags.FirstOrDefault(t => t.Key == key);
            if (tag == null)
            {
                Add(key, value);
            }
            else
            {
                tag.Value = value;
            }
        }
    }

    public ICollection<string> Keys => _tags.Select(t => t.Key).ToList();
    public ICollection<IIrcTagValue> Values => _tags.Select(t => t.Value).ToList();
}