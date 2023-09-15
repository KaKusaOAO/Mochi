using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mochi.Nbt;

public class NbtList : NbtTag, IList<NbtTag>
{
    public NbtList() : base(TagType.List)
    {
    }

    public NbtList(TagType type) : this() => ContentType = type;

    public NbtList(TagType type, List<NbtTag> list) : this()
    {
        ContentType = type;
        _children = list;
    }

    public NbtList(List<NbtTag> list) : this()
    {
        if (list.Any())
        {
            ContentType = list.First().Type;
        }

        _children = list;
    }

    public TagType ContentType { get; private set; }

    public int Count => _children.Count;

    public bool IsReadOnly => false;

    public NbtTag this[int index]
    {
        get => _children[index];
        set => _children[index] = value;
    }

    public int IndexOf(NbtTag item) => _children.IndexOf(item);

    public void Insert(int index, NbtTag item)
    {
        _children.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _children.RemoveAt(index);
    }

    public void Add(NbtTag item)
    {
        _children.Add(item);
    }

    public void Clear()
    {
        _children.Clear();
    }

    public bool Contains(NbtTag item) => _children.Contains(item);

    public void CopyTo(NbtTag[] array, int arrayIndex)
    {
        _children.CopyTo(array, arrayIndex);
    }

    public bool Remove(NbtTag item) => _children.Remove(item);

    public IEnumerator<NbtTag> GetEnumerator() => _children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _children.GetEnumerator();

    private readonly List<NbtTag> _children = new();

    public static NbtList Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var result = new NbtList();
        InternalDeserializeReadTagName(buffer, ref index, named, TagType.List, result);
        var type = (TagType)NbtIO.ReadByte(buffer, ref index);
        result.ContentType = type;
        var count = NbtIO.ReadInt(buffer, ref index);
        for (var i = 0; i < count; i++) result.Add(Deserialize(buffer, ref index, false, type));
        return result;
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        var result = $"TAG_List({name}): {Count} entries\n{{";

        foreach (var tag in _children)
        {
            var lines = tag.ToString().Split('\n');
            foreach (var line in lines) result += "\n  " + line.ToString();
        }

        return result + "\n}";
    }
}