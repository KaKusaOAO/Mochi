namespace Mochi.Nbt;

public class NbtShort : NbtTag, INbtValue<short>
{
    public NbtShort() : base(TagType.Short)
    {
    }

    public NbtShort(short n) : this() => Value = n;

    public short Value { get; set; }
    
    public static implicit operator NbtShort(short n) => new(n);

    public static NbtShort Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var result = new NbtShort();
        InternalDeserializeReadTagName(buffer, ref index, named, TagType.Short, result);
        result.Value = NbtIO.ReadShort(buffer, ref index);
        return result;
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Short({name}): {Value}";
    }
}