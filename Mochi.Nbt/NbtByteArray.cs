namespace Mochi.Nbt;

public class NbtByteArray : NbtTag, INbtValue<byte[]>
{
    public NbtByteArray() : base(TagType.ByteArray)
    {
    }

    public NbtByteArray(byte[] arr) : this() => Value = arr;

    public byte[] Value { get; }
    
    public static implicit operator NbtByteArray(byte[] arr) => new(arr);

    public static NbtByteArray Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var name = InternalDeserializeReadTagName(buffer, ref index, named, TagType.ByteArray);
        return new NbtByteArray(NbtIO.ReadByteArray(buffer, ref index))
        {
            Name = name
        };
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Byte_Array({name}): [{Value.Length} bytes]";
    }
}