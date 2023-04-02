﻿namespace Mochi.Nbt;

public class NbtInt : NbtTag, INbtValue<int>
{
    public NbtInt() : base(3)
    {
    }

    public NbtInt(int n) : base(3) => Value = n;

    public int Value { get; set; }

    public static NbtInt Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var result = new NbtInt();
        InternalDeserializeReadTagName(buffer, ref index, named, TagType.Int, result);
        result.Value = NbtIO.ReadInt(buffer, ref index);
        return result;
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Int({name}): {Value}";
    }
}