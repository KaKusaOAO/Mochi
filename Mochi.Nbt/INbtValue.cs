namespace Mochi.Nbt;

public interface INbtValue<out T>
{
    T Value { get; }
}