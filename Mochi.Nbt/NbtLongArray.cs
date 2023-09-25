using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtLongArray : NbtValue<long[]>, INbtCollection<NbtLong>
{
    private readonly List<long> _list = new();
    
    public override TagTypeInfo TypeInfo => TagTypeInfo.LongArray;
    public TagTypeInfo ElementTypeInfo => TagTypeInfo.Long;

    public int Count => _list.Count;

    bool ICollection<NbtLong>.IsReadOnly => false;

    public override long[] Value => _list.ToArray();

    public NbtLong this[int index]
    {
        get => NbtLong.CreateValue(_list[index]);
        set => _list[index] = value.Value;
    }

    public NbtLongArray(IEnumerable<long> bytes)
    {
        _list.AddRange(bytes);
    }

    public override void WriteContentTo(NbtWriter writer)
    {
        var arr = Value;
        writer.WriteInt32(arr.Length);
        
        foreach (var i in arr)
        {
            writer.WriteInt64(i);
        }
    }

    public IEnumerator<NbtLong> GetEnumerator() => _list.Select(NbtLong.CreateValue).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(NbtLong tag)
    {
        _list.Add(tag.Value);
    }

    public void Clear() => _list.Clear();

    public bool Contains(NbtLong tag) => _list.Contains(tag.Value);

    void ICollection<NbtLong>.CopyTo(NbtLong[] array, int arrayIndex)
    {
        var arr = _list.Select(NbtLong.CreateValue).ToArray();
        Array.Copy(arr, 0, array, arrayIndex, arr.Length);
    }

    public bool Remove(NbtLong tag) => _list.Remove(tag.Value);

    public int IndexOf(NbtLong tag) => _list.IndexOf(tag.Value);

    public void Insert(int index, NbtLong tag) => _list.Insert(index, tag.Value);

    public void RemoveAt(int index) => _list.RemoveAt(index);

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

    public override void Accept(ITagVisitor visitor) => visitor.VisitLongArray(this);

    public sealed class LongArrTypeInfo : TagTypeInfo<NbtLongArray>
    {
        internal static LongArrTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.LongArray;

        public override string FriendlyName => "TAG_LongArray";

        private LongArrTypeInfo() {}

        protected override NbtLongArray LoadValue(NbtReader reader)
        {
            var size = reader.ReadInt32();
            var arr = new long[size];
            
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = reader.ReadInt64();
            }
            
            return new NbtLongArray(arr);
        }
    }
}