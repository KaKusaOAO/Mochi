using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtList : NbtTag, INbtCollection<NbtTag>
{
    private readonly List<NbtTag> _list = new();
    
    public override TagTypeInfo TypeInfo => TagTypeInfo.List;

    public TagTypeInfo ElementTypeInfo { get; private set; }

    public int Count => _list.Count;

    bool ICollection<NbtTag>.IsReadOnly => false;
    
    public NbtTag this[int index]
    {
        get => _list[index];
        set
        {
            if (!SetTag(index, value))
                throw new InvalidOperationException(
                    $"Trying to insert tag of type {value.Type} to list of {ElementTypeInfo.Type}");
        }
    }
    
    public NbtList(IEnumerable<NbtTag> list, TagType type)
    {
        ElementTypeInfo = TagTypeInfo.GetTagType(type);
        _list.AddRange(list);
    }
    
    public NbtList() : this(Enumerable.Empty<NbtTag>(), TagType.End) {}

    public override void WriteContentTo(NbtWriter writer)
    {
        var list = _list.ToArray();
        
        if (!list.Any())
        {
            writer.WriteTagType(TagType.End);
            writer.WriteInt32(0);
            return;
        }

        if (list.Select(t => t.Type).Distinct().Count() > 1)
            throw new Exception("NBT tags in the list has different types");

        var type = list.First().Type;
        if (type != ElementTypeInfo.Type)
            throw new Exception("Saved element type is not equal to the actual type of the items in the list");
        
        writer.WriteTagType(type);
        writer.WriteInt32(list.Length);

        foreach (var tag in _list)
        {
            tag.WriteContentTo(writer);
        }
    }

    public IEnumerator<NbtTag> GetEnumerator() => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void UpdateTypeAfterRemove()
    {
        if (_list.Any()) return;
        ElementTypeInfo = TagTypeInfo.End;
    }

    private bool UpdateType(NbtTag tag)
    {
        if (tag.Type == TagType.End)
        {
            // Attempting to add an end tag
            return false;
        }

        if (ElementTypeInfo.Type == TagType.End)
        {
            ElementTypeInfo = tag.TypeInfo;
            return true;
        }

        return tag.Type == ElementTypeInfo.Type;
    }

    public bool SetTag(int index, NbtTag tag)
    {
        if (!UpdateType(tag)) return false;

        _list[index] = tag;
        return true;
    }

    public bool AddTag(NbtTag tag)
    {
        if (!UpdateType(tag)) return false;
        
        _list.Add(tag);
        return true;
    }

    public bool InsertTag(int index, NbtTag tag)
    {
        if (!UpdateType(tag)) return false;
        
        _list.Insert(index, tag);
        return true;
    }

    public void Add(NbtTag item)
    {
        if (!AddTag(item))
            throw new InvalidOperationException(
                $"Trying to add tag of type {item.Type} to list of {ElementTypeInfo.Type}");
    }

    public void Clear()
    {
        _list.Clear();
        UpdateTypeAfterRemove();
    }

    public bool Contains(NbtTag item) => _list.Contains(item);

    void ICollection<NbtTag>.CopyTo(NbtTag[] array, int arrayIndex) => 
        ((ICollection<NbtTag>) _list).CopyTo(array, arrayIndex);

    public bool Remove(NbtTag item)
    {
        if (!_list.Remove(item)) return false;
        
        UpdateTypeAfterRemove();
        return true;
    }

    public int IndexOf(NbtTag item) => _list.IndexOf(item);

    public void Insert(int index, NbtTag item)
    {
        if (!InsertTag(index, item))
            throw new InvalidOperationException(
                $"Trying to insert tag of type {item.Type} to list of {ElementTypeInfo.Type}");
    }

    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
        UpdateTypeAfterRemove();
    }

    public override void Accept(ITagVisitor visitor) => visitor.VisitList(this);

    public sealed class ListTypeInfo : TagTypeInfo<NbtList>
    {
        internal static ListTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.List;

        public override string FriendlyName => "TAG_List";

        private ListTypeInfo() {}

        protected override NbtList LoadValue(NbtReader reader)
        {
            reader.PushDepth();

            try
            {
                var type = reader.ReadTagType();
                var count = reader.ReadInt32();

                if (type == TagType.End && count > 0)
                    throw new MalformedNbtException("Missing type on NbtList");

                var list = new List<NbtTag>();
                for (var i = 0; i < count; i++)
                {
                    list.Add(GetTagType(type).Load(reader));
                }

                return new NbtList(list, type);
            }
            finally
            {
                reader.PopDepth();
            }
        }
    }
}