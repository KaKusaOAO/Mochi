namespace Mochi.Nbt;

public class NbtTooComplexException : NbtIOException
{
    public NbtTooComplexException(string message) : base(message) { }
}