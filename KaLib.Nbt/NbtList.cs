using System.Collections;
using System.Collections.Generic;

namespace KaLib.Nbt;

public class NbtList : NbtTag, IList<NbtTag>
{
    public NbtList() : base(9)
    {
    }

    public NbtList(TagType type) : base(9) => ContentType = type;

    public NbtList(TagType type, List<NbtTag> list) : base(9)
    {
        ContentType = type;
        children = list;
    }

    public TagType ContentType { get; set; }

    public int Count => children.Count;

    public bool IsReadOnly => false;

    public NbtTag this[int index]
    {
        get => children[index];
        set => children[index] = value;
    }

    public int IndexOf(NbtTag item) => children.IndexOf(item);

    public void Insert(int index, NbtTag item)
    {
        children.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        children.RemoveAt(index);
    }

    public void Add(NbtTag item)
    {
        children.Add(item);
    }

    public void Clear()
    {
        children.Clear();
    }

    public bool Contains(NbtTag item) => children.Contains(item);

    public void CopyTo(NbtTag[] array, int arrayIndex)
    {
        children.CopyTo(array, arrayIndex);
    }

    public bool Remove(NbtTag item) => children.Remove(item);

    public IEnumerator<NbtTag> GetEnumerator() => children.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => children.GetEnumerator();

    private List<NbtTag> children = new List<NbtTag>();

    public static NbtList Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var result = new NbtList();
        InternalDeserializePhaseA(buffer, ref index, named, TagType.List, result);
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

        foreach (var tag in children)
        {
            var lines = tag.ToString().Split('\n');
            foreach (var line in lines) result += "\n  " + line.ToString();
        }

        return result + "\n}";
    }
}