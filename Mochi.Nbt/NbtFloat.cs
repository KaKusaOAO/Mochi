using System;

namespace Mochi.Nbt;

public class NbtFloat : NbtTag, INbtValue<float>, INbtNumeric
{
    public NbtFloat() : base(TagType.Float)
    {
    }

    public NbtFloat(float f) : this() => Value = f;

    public float Value { get; }
    
    public static implicit operator NbtFloat(float f) => new(f);

    public static NbtFloat Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var name = InternalDeserializeReadTagName(buffer, ref index, named, TagType.Float);
        return new NbtFloat(NbtIO.ReadFloat(buffer, ref index))
        {
            Name = name
        };
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Float({name}): {Value}";
    }

    public long AsInt64() => (long) Math.Floor(Value);

    public int AsInt32() => (int) Math.Floor(Value);

    public short AsInt16() => (short) ((int) Math.Floor(Value) & 0xffff);

    public byte AsByte() => (byte) ((int) Math.Floor(Value) & 0xff);

    public double AsDouble() => Value;

    public float AsSingle() => Value;

    public decimal AsDecimal() => (decimal) Value;
}