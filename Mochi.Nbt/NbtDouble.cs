namespace Mochi.Nbt;

public class NbtDouble : NbtTag, INbtValue<double>
{
    public NbtDouble() : base(TagType.Double)
    {
    }

    public NbtDouble(double d) : this() => Value = d;

    public double Value { get; set; }
    
    public static implicit operator NbtDouble(double d) => new(d);

    public static NbtDouble Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var result = new NbtDouble();
        InternalDeserializeReadTagName(buffer, ref index, named, TagType.Double, result);
        result.Value = NbtIO.ReadDouble(buffer, ref index);
        return result;
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Double({name}): {Value}";
    }
}