namespace Mochi.Nbt;

public class NbtInt : NbtTag, INbtValue<int>, INbtNumeric
{
    public NbtInt() : base(TagType.Int)
    {
    }

    public NbtInt(int n) : this() => Value = n;

    public int Value { get; }
    
    public static implicit operator NbtInt(int n) => new(n);

    public static NbtInt Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var name = InternalDeserializeReadTagName(buffer, ref index, named, TagType.Int);
        return new NbtInt(NbtIO.ReadInt(buffer, ref index))
        {
            Name = name
        };
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Int({name}): {Value}";
    }

    public long AsInt64() => Value;

    public int AsInt32() => Value;

    public short AsInt16() => (short) (Value & 0xffff);

    public byte AsByte() => (byte) (Value & 0xff);

    public double AsDouble() => Value;

    public float AsSingle() => Value;

    public decimal AsDecimal() => Value;
}