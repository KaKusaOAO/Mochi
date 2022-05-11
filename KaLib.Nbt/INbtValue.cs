namespace KaLib.Nbt
{
    public interface INbtValue<T>
    {
        T Value { get; }
    }
}