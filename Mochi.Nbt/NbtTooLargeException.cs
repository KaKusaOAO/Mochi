namespace Mochi.Nbt;

public class NbtTooLargeException : NbtIOException
{
    public NbtTooLargeException(string message) : base(message) { }
}