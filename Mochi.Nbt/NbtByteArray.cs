using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtByteArray : NbtValue<byte[]>, INbtCollection<NbtByte>
{
    private readonly List<byte> _list = new();
    
    public override TagTypeInfo TypeInfo => TagTypeInfo.ByteArray;
    public TagTypeInfo ElementTypeInfo => TagTypeInfo.Byte;

    public override byte[] Value => _list.ToArray();

    public NbtByteArray(IEnumerable<byte> bytes)
    {
        _list.AddRange(bytes);
    }

    public override void WriteContentTo(NbtWriter writer)
    {
        var arr = Value;
        writer.WriteInt32(arr.Length);
        writer.WriteBytes(arr);
    }

    public IEnumerator<NbtByte> GetEnumerator() => _list.Select(NbtByte.CreateValue).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(NbtByte tag)
    {
        _list.Add(tag.Value);
    }

    public void Clear() => _list.Clear();

    public bool Contains(NbtByte tag) => _list.Contains(tag.Value);

    void ICollection<NbtByte>.CopyTo(NbtByte[] array, int arrayIndex)
    {
        var arr = _list.Select(NbtByte.CreateValue).ToArray();
        Array.Copy(arr, 0, array, arrayIndex, arr.Length);
    }

    public bool Remove(NbtByte tag) => _list.Remove(tag.Value);

    public int Count => _list.Count;

    bool ICollection<NbtByte>.IsReadOnly => false;

    public int IndexOf(NbtByte tag) => _list.IndexOf(tag.Value);

    public void Insert(int index, NbtByte tag) => _list.Insert(index, tag.Value);

    public void RemoveAt(int index) => _list.RemoveAt(index);

    public NbtByte this[int index]
    {
        get => NbtByte.CreateValue(_list[index]);
        set => _list[index] = value.Value;
    }

    public bool SetTag(int index, NbtTag tag)
    {
        if (tag is not INbtNumeric numeric)
            return false;
        
        _list[index] = numeric.AsByte();
        return true;
    }

    public bool AddTag(NbtTag tag)
    {
        if (tag is not INbtNumeric numeric)
            return false;
        
        _list.Add(numeric.AsByte());
        return true;
    }

    public bool InsertTag(int index, NbtTag tag)
    {
        if (tag is not INbtNumeric numeric)
            return false;
        
        _list.Insert(index, numeric.AsByte());
        return true;
    }

    public override void Accept(ITagVisitor visitor) => visitor.VisitByteArray(this);

    public sealed class ByteArrTypeInfo : TagTypeInfo<NbtByteArray>
    {
        internal static ByteArrTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.ByteArray;

        public override string FriendlyName => "TAG_ByteArray";

        private ByteArrTypeInfo() {}

        protected override NbtByteArray LoadValue(NbtReader reader)
        {
            var size = reader.ReadInt32();
            var arr = reader.ReadFixedLength(size);
            return new NbtByteArray(arr);
        }
    }
}