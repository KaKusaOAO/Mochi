using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtIntArray : NbtValue<int[]>, INbtCollection<NbtInt>
{
    private readonly List<int> _list = new();
    
    public override TagTypeInfo TypeInfo => TagTypeInfo.IntArray;
    public TagTypeInfo ElementTypeInfo => TagTypeInfo.Int;

    public override int[] Value => _list.ToArray();

    public int Count => _list.Count;

    bool ICollection<NbtInt>.IsReadOnly => false;

    public NbtInt this[int index]
    {
        get => NbtInt.CreateValue(_list[index]);
        set => _list[index] = value.Value;
    }
    
    public NbtIntArray(IEnumerable<int> bytes)
    {
        _list.AddRange(bytes);
    }

    public override void WriteContentTo(NbtWriter writer)
    {
        var arr = Value;
        writer.WriteInt32(arr.Length);
        
        foreach (var i in arr)
        {
            writer.WriteInt32(i);
        }
    }

    public IEnumerator<NbtInt> GetEnumerator() => _list.Select(NbtInt.CreateValue).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(NbtInt tag)
    {
        _list.Add(tag.Value);
    }

    public void Clear() => _list.Clear();

    public bool Contains(NbtInt tag) => _list.Contains(tag.Value);

    void ICollection<NbtInt>.CopyTo(NbtInt[] array, int arrayIndex)
    {
        var arr = _list.Select(NbtInt.CreateValue).ToArray();
        Array.Copy(arr, 0, array, arrayIndex, arr.Length);
    }

    public bool Remove(NbtInt tag) => _list.Remove(tag.Value);

    public int IndexOf(NbtInt tag) => _list.IndexOf(tag.Value);

    public void Insert(int index, NbtInt tag) => _list.Insert(index, tag.Value);

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

    public override void Accept(ITagVisitor visitor) => visitor.VisitIntArray(this);

    public sealed class IntArrTypeInfo : TagTypeInfo<NbtIntArray>
    {
        internal static IntArrTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.IntArray;

        public override string FriendlyName => "TAG_IntArray";

        private IntArrTypeInfo() {}

        protected override NbtIntArray LoadValue(NbtReader reader)
        {
            var size = reader.ReadInt32();
            var arr = new int[size];
            
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = reader.ReadInt32();
            }
            
            return new NbtIntArray(arr);
        }
    }
}