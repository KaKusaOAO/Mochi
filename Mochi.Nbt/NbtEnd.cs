namespace Mochi.Nbt;

public class NbtEnd : NbtTag
{
    private static NbtEnd? _instance;
    public static NbtEnd Shared => _instance ??= new NbtEnd();
    
    public NbtEnd() : base(TagType.End)
    {
    }

    public override string ToString()
    {
        var name = Name == null ? "None" : $"'{Name}'";
        return $"TAG_End({name})";
    }
}