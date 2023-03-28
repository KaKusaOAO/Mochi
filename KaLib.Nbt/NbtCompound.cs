using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace KaLib.Nbt;

public class NbtCompound : NbtTag, IDictionary<string, NbtTag>
{
    public NbtTag this[string key]
    {
        get => children.Find(tag => tag.Name == key);
        set
        {
            // Should I respect C# standard, which throws error
            // when old item is not found?

            var old = this[key];
            if (old != null) children.Remove(old);

            value.Name = key;
            children.Add(value);
        }
    }

    public ICollection<string> Keys => children.ConvertAll(tag => tag.Name);

    public ICollection<NbtTag> Values => children.ConvertAll(t => t);

    public int Count => children.Count;

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

    public bool GetBool(string key) => this[key].As<NbtByte>().AsBool;

    public int GetInt(string key) => this[key].As<NbtInt>().Value;

    public string GetString(string key) => this[key].As<NbtString>().Value;

    public double GetDouble(string key) => this[key].As<NbtDouble>().Value;

    public float GetFloat(string key) => this[key].As<NbtFloat>().Value;

    public short GetShort(string key) => this[key].As<NbtShort>().Value;

    public long GetLong(string key) => this[key].As<NbtLong>().Value;

    public void Clear()
    {
        children.Clear();
    }

    public bool Contains(KeyValuePair<string, NbtTag> item) => ContainsKey(item.Key);

    public bool ContainsKey(string key) => this[key] != null;

    public void CopyTo(KeyValuePair<string, NbtTag>[] array, int arrayIndex)
    {
        foreach (var pair in children.ConvertAll(t => new KeyValuePair<string, NbtTag>(t.Name, t)))
            array[arrayIndex++] = pair;
    }

    public IEnumerator<KeyValuePair<string, NbtTag>> GetEnumerator()
    {
        return children.ConvertAll(t => new KeyValuePair<string, NbtTag>(t.Name, t)).GetEnumerator();
    }

    public bool Remove(string key) => children.Remove(this[key]);

    public bool Remove(KeyValuePair<string, NbtTag> item) => Remove(item.Key);

#if NETCOREAPP
        public bool TryGetValue(string key, [MaybeNullWhen(false)] out NbtTag value)
#else
    public bool TryGetValue(string key, out NbtTag value)
#endif
    {
        value = this[key];
        return value != null;
        ;
    }

    IEnumerator IEnumerable.GetEnumerator() => children.GetEnumerator();

    private List<NbtTag> children = new List<NbtTag>();

    public NbtCompound() : base(10)
    {
    }

    public static NbtCompound Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var result = new NbtCompound();

        InternalDeserializePhaseA(buffer, ref index, named, TagType.Compound, result);

        while (buffer[index] != 0)
        {
            var tag = NbtTag.Deserialize(buffer, ref index, true);
            result.Add(tag.Name, tag);
        }

        index++;

        return result;
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        var result = $"TAG_Compound({name}): {Count} entries\n{{";

        foreach (var tag in Values)
        {
            var lines = tag.ToString().Split('\n');
            foreach (var line in lines) result += "\n  " + line.ToString();
        }

        return result + "\n}";
    }
}