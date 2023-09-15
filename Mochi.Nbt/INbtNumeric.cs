namespace Mochi.Nbt;

public interface INbtNumeric
{
    public long AsInt64();
    public int AsInt32();
    public short AsInt16();
    public byte AsByte();
    public double AsDouble();
    public float AsSingle();
    public decimal AsDecimal();
}