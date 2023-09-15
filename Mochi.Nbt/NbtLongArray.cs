namespace Mochi.Nbt;

public class NbtLongArray : NbtTag, INbtValue<long[]>
{
    public NbtLongArray() : base(TagType.LongArray)
    {
    }

    public NbtLongArray(long[] n) : this() => Value = n;

    public long[] Value { get; }
    
    public static implicit operator NbtLongArray(long[] n) => new(n);

    public static NbtLongArray Deserialize(byte[] buffer, ref int index, bool named = false)
    {
        var name = InternalDeserializeReadTagName(buffer, ref index, named, TagType.LongArray);
        var count = NbtIO.ReadInt(buffer, ref index);
        var arr = new long[count];
        for (var i = 0; i < count; i++) arr[i] = NbtIO.ReadLong(buffer, ref index);

        return new NbtLongArray(arr)
        {
            Name = name
        };
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_Long_Array({name}): [{Value.Length} items]";
    }
}