﻿namespace Mochi.Nbt;

public class NbtLongArray : NbtTag, INbtValue<long[]>
{
    public NbtLongArray() : base(TagType.LongArray)
    {
    }

    public NbtLongArray(long[] n) : this() => Value = n;

    public long[] Value { get; set; }
    
    public static implicit operator NbtLongArray(long[] n) => new(n);

    public static NbtLongArray Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var result = new NbtLongArray();
        InternalDeserializeReadTagName(buffer, ref index, named, TagType.LongArray, result);
        var count = NbtIO.ReadInt(buffer, ref index);

        result.Value = new long[count];
        for (var i = 0; i < count; i++) result.Value[i] = NbtIO.ReadLong(buffer, ref index);

        return result;
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Long_Array({name}): [{Value.Length} items]";
    }
}