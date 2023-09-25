namespace Mochi.Nbt;

public interface INbtNumeric
{
    public byte AsByte();
    public short AsInt16();
    public int AsInt32();
    public long AsInt64();
    public ushort AsUInt16();
    public uint AsUInt32();
    public ulong AsUInt64();
    public float AsSingle();
    public double AsDouble();
}