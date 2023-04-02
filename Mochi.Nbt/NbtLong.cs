namespace Mochi.Nbt;

public class NbtLong : NbtTag, INbtValue<long>
{
    public NbtLong() : base(TagType.Long)
    {
    }

    public NbtLong(long n) : this() => Value = n;

    public long Value { get; set; }
    
    public static implicit operator NbtLong(long n) => new(n);

    public static NbtLong Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var result = new NbtLong();
        InternalDeserializeReadTagName(buffer, ref index, named, TagType.Long, result);
        result.Value = NbtIO.ReadLong(buffer, ref index);
        return result;
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Long({name}): {Value}L";
    }
}