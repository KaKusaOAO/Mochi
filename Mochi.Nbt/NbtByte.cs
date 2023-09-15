namespace Mochi.Nbt;

public class NbtByte : NbtTag, INbtValue<byte>, INbtNumeric
{
    public NbtByte() : base(TagType.Byte)
    {
    }

    public NbtByte(byte value) : this() => Value = value;

    public NbtByte(bool flag) : this((byte)(flag ? 1 : 0))
    {
    }
    
    public static implicit operator NbtByte(byte value) => new(value);
    public static implicit operator NbtByte(bool flag) => new(flag);

    public byte Value { get; }

    public bool AsBool() => Value != 0;

    public static NbtByte Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var name = InternalDeserializeReadTagName(buffer, ref index, named, TagType.Byte);
        return new NbtByte(NbtIO.ReadByte(buffer, ref index))
        {
            Name = name
        };
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Byte({name}): {Value}";
    }

    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => Value.GetHashCode();

    public override bool Equals(object obj)
    {
        if (!(obj is NbtByte b)) return false;
        return Value == b.Value;
    }

    public long AsInt64() => Value;

    public int AsInt32() => Value;

    public short AsInt16() => Value;

    public byte AsByte() => Value;

    public double AsDouble() => Value;

    public float AsSingle() => Value;

    public decimal AsDecimal() => Value;
}