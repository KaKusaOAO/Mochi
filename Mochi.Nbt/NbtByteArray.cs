namespace Mochi.Nbt;

public class NbtByteArray : NbtTag, INbtValue<byte[]>
{
    public NbtByteArray() : base(TagType.ByteArray)
    {
    }

    public NbtByteArray(byte[] arr) : this() => Value = arr;

    public byte[] Value { get; set; }
    
    public static implicit operator NbtByteArray(byte[] arr) => new(arr);

    public static NbtByteArray Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var result = new NbtByteArray();
        InternalDeserializeReadTagName(buffer, ref index, named, TagType.ByteArray, result);
        result.Value = NbtIO.ReadByteArray(buffer, ref index);
        return result;
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Byte_Array({name}): [{Value.Length} bytes]";
    }
}