namespace KaLib.Nbt;

public class NbtByte : NbtTag, INbtValue<byte>
{
    public NbtByte() : base(1)
    {
    }

    public NbtByte(byte value) : base(1) => Value = value;

    public NbtByte(bool flag) : this((byte)(flag ? 1 : 0))
    {
    }

    public byte Value { get; set; }

    public bool AsBool => Value != 0;

    public static NbtByte Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var result = new NbtByte();
        InternalDeserializePhaseA(buffer, ref index, named, TagType.Byte, result);
        result.Value = NbtIO.ReadByte(buffer, ref index);
        return result;
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
}