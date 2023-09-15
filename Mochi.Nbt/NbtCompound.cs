using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Mochi.Nbt;

public class NbtCompound : NbtTag, IDictionary<string, NbtTag>
{
    public NbtTag? this[string key]
    {
        get => _children.Find(tag => tag.Name == key);
        set
        {
            // Should I respect C# standard, which throws error
            // when old item is not found?

            var old = this[key];
            if (old != null) _children.Remove(old);
            if (value == null) return;
            
            value.Name = key;
            _children.Add(value);
        }
    }

    public ICollection<string> Keys => _children.Select(tag => tag.Name!).ToList();

    public ICollection<NbtTag> Values => _children.ToList();

    public int Count => _children.Count;

    public bool IsReadOnly => false;

    public void Add(string key, NbtTag value)
    {
        this[key] = value;
        value.Name = key;
    }

    public void AddBool(string key, bool val) => Add(key, new NbtByte(val ? (byte)1 : (byte)0));

    public void AddInt(string key, int val) => Add(key, new NbtInt(val));

    public void AddString(string key, string val) => Add(key, new NbtString(val));

    public void AddDouble(string key, double val) => Add(key, new NbtDouble(val));

    public void AddLong(string key, long val) => Add(key, new NbtLong(val));

    public void Add(KeyValuePair<string, NbtTag> item) => Add(item.Key, item.Value);

    public bool GetBool(string key) => this[key]!.As<NbtByte>().AsBool();
    public byte GetByte(string key) => this[key]!.As<NbtByte>().Value;

    public int GetInt(string key) => this[key]!.As<NbtInt>().Value;

    public string GetString(string key) => this[key]!.As<NbtString>().Value;

    public double GetDouble(string key) => this[key]!.As<NbtDouble>().Value;

    public float GetFloat(string key) => this[key]!.As<NbtFloat>().Value;

    public short GetShort(string key) => this[key]!.As<NbtShort>().Value;

    public long GetLong(string key) => this[key]!.As<NbtLong>().Value;

    public void Clear()
    {
        _children.Clear();
    }

    public bool Contains(KeyValuePair<string, NbtTag> item) => ContainsKey(item.Key);

    public bool ContainsKey(string key) => this[key] != null;

    public void CopyTo(KeyValuePair<string, NbtTag>[] array, int arrayIndex)
    {
        foreach (var pair in _children.Select(t => new KeyValuePair<string, NbtTag>(t.Name!, t)))
            array[arrayIndex++] = pair;
    }

    public IEnumerator<KeyValuePair<string, NbtTag>> GetEnumerator()
    {
        return _children.Select(t => new KeyValuePair<string, NbtTag>(t.Name!, t)).GetEnumerator();
    }

    public bool Remove(string key) => ContainsKey(key) && _children.Remove(this[key]!);

    public bool Remove(KeyValuePair<string, NbtTag> item) => Remove(item.Key);

#pragma warning disable CS8767
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out NbtTag value)
#pragma warning restore CS8767
    {
        value = this[key];
        return value != null;
    }

    IEnumerator IEnumerable.GetEnumerator() => _children.GetEnumerator();

    private readonly List<NbtTag> _children = new();

    public NbtCompound() : base(TagType.Compound)
    {
    }

    public static NbtCompound Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var result = new NbtCompound();
        InternalDeserializeReadTagName(buffer, ref index, named, TagType.Compound, result);

        while (buffer[index] != 0)
        {
            var tag = NbtTag.Deserialize(buffer, ref index, true);
            result.Add(tag.Name!, tag);
        }

        index++;
        return result;
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        var result = $"TAG_Compound({name}): {Count} entries\n{{";

        var sb = new StringBuilder();
        foreach (var tag in Values)
        {
            var lines = tag.ToString().Split('\n');
            foreach (var line in lines) sb.Append("\n  " + line);
        }

        return result + sb + "\n}";
    }
}