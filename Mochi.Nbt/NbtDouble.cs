using System;

namespace Mochi.Nbt;

public class NbtDouble : NbtTag, INbtValue<double>, INbtNumeric
{
    public NbtDouble() : base(TagType.Double)
    {
    }

    public NbtDouble(double d) : this() => Value = d;

    public double Value { get; }
    
    public static implicit operator NbtDouble(double d) => new(d);

    public static NbtDouble Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var name = InternalDeserializeReadTagName(buffer, ref index, named, TagType.Double);
        return new NbtDouble(NbtIO.ReadDouble(buffer, ref index))
        {
            Name = name
        };
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Double({name}): {Value}";
    }

    public long AsInt64() => (long) Math.Floor(Value);

    public int AsInt32() => (int) Math.Floor(Value);

    public short AsInt16() => (short) ((int) Math.Floor(Value) & 0xffff);

    public byte AsByte() => (byte) ((int) Math.Floor(Value) & 0xff);

    public double AsDouble() => Value;

    public float AsSingle() => (float) Value;

    public decimal AsDecimal() => (decimal) Value;
}