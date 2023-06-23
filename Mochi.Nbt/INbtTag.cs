namespace Mochi.Nbt;

public interface INbtTag
{
    public byte RawType { get; }
    public string? Name { get; set; }
}