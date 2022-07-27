namespace KaLib.Nbt
{
    public interface INbtValue<out T>
    {
        T Value { get; }
    }
}