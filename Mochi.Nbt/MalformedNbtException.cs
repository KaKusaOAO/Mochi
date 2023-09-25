namespace Mochi.Nbt;

public class MalformedNbtException : NbtIOException
{
    public MalformedNbtException(string message) : base(message) { }
}