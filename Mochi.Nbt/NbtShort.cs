namespace Mochi.Nbt;

public class NbtShort : NbtTag, INbtValue<short>, INbtNumeric
{
    public NbtShort() : base(TagType.Short)
    {
    }

    public NbtShort(short n) : this() => Value = n;

    public short Value { get; set; }
    
    public static implicit operator NbtShort(short n) => new(n);

    public static NbtShort Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var name = InternalDeserializeReadTagName(buffer, ref index, named, TagType.Short);
        return new NbtShort(NbtIO.ReadShort(buffer, ref index))
        {
            Name = name
        };
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Short({name}): {Value}";
    }

    public long AsInt64() => Value;

    public int AsInt32() => Value;

    public short AsInt16() => Value;

    public byte AsByte() => (byte) (Value & 0xff);

    public double AsDouble() => Value;

    public float AsSingle() => Value;

    public decimal AsDecimal() => Value;
}