using System.Collections;
using System.Collections.Generic;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtCompound : NbtTag, IDictionary<string, NbtTag>
{
    private readonly Dictionary<string, NbtTag> _tags = new();

    public override TagTypeInfo TypeInfo => TagTypeInfo.Compound;

    public ICollection<string> Keys => _tags.Keys;

    public ICollection<NbtTag> Values => _tags.Values;

    public int Count => _tags.Count;

    public override NbtTag? this[string key]
    {
#pragma warning disable CS8766
        get => _tags.GetValueOrDefault(key);
#pragma warning restore CS8766
        set
        {
            if (value == null) Remove(key);
            else _tags[key] = value;
        }
    }

    public NbtCompound(Dictionary<string, NbtTag> tags)
    {
        foreach (var (key, value) in tags)
        {
            _tags.Add(key, value);
        }
    }
    
    public NbtCompound() {}

    public override NbtCompound AsCompound() => this;

    public override void WriteContentTo(NbtWriter writer)
    {
        foreach (var (key, value) in _tags)
        {
            writer.WriteTagType(value.Type);
            writer.WriteUtf(key);
            value.WriteContentTo(writer);
        }
        
        writer.WriteTagType(TagType.End);
    }

    public IEnumerator<KeyValuePair<string, NbtTag>> GetEnumerator() => _tags.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void ICollection<KeyValuePair<string, NbtTag>>.Add(KeyValuePair<string, NbtTag> item) => 
        _tags.Add(item.Key, item.Value);

    public void Clear() => _tags.Clear();

    bool ICollection<KeyValuePair<string, NbtTag>>.Contains(KeyValuePair<string, NbtTag> item) => 
        ((ICollection<KeyValuePair<string, NbtTag>>) _tags).Contains(item);

    void ICollection<KeyValuePair<string, NbtTag>>.CopyTo(KeyValuePair<string, NbtTag>[] array, int arrayIndex) => 
        ((ICollection<KeyValuePair<string, NbtTag>>) _tags).CopyTo(array, arrayIndex);

    bool ICollection<KeyValuePair<string, NbtTag>>.Remove(KeyValuePair<string, NbtTag> item) =>
        ((ICollection<KeyValuePair<string, NbtTag>>) _tags).Remove(item);

    bool ICollection<KeyValuePair<string, NbtTag>>.IsReadOnly => false;

    public void Add(string key, NbtTag value) => _tags.Add(key, value);

    public bool ContainsKey(string key) => _tags.ContainsKey(key);

    public bool Remove(string key) => _tags.Remove(key);

    public bool TryGetValue(string key, out NbtTag value) => _tags.TryGetValue(key, out value!);

    public override void Accept(ITagVisitor visitor) => visitor.VisitCompound(this);

    public sealed class CompoundTypeInfo : TagTypeInfo<NbtCompound>
    {
        internal static CompoundTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.Compound;

        public override string FriendlyName => "TAG_Compound";

        private CompoundTypeInfo() {}

        protected override NbtCompound LoadValue(NbtReader reader)
        {
            var dict = new Dictionary<string, NbtTag>();
            
            TagType type;
            while ((type = reader.ReadTagType()) != TagType.End)
            {
                var key = reader.ReadUtf();
                var value = GetTagType(type).Load(reader);
                dict[key] = value;
            }

            return new NbtCompound(dict);
        }
    }
}