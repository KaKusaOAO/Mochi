namespace Mochi.Nbt;

public class NbtFloat : NbtTag, INbtValue<float>
{
    public NbtFloat() : base(TagType.Float)
    {
    }

    public NbtFloat(float f) : this() => Value = f;

    public float Value { get; set; }
    
    public static implicit operator NbtFloat(float f) => new(f);

    public static NbtFloat Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var result = new NbtFloat();
        InternalDeserializeReadTagName(buffer, ref index, named, TagType.Float, result);
        result.Value = NbtIO.ReadFloat(buffer, ref index);
        return result;
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Float({name}): {Value}";
    }
}