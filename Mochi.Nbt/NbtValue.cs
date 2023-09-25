using System;

namespace Mochi.Nbt;

public abstract class NbtValue : NbtTag
{
    public override NbtValue AsValue() => this;
}

public abstract class NbtValue<T> : NbtValue
{
    public abstract T Value { get; }

    public override TValue GetValue<TValue>()
    {
        if (Value is TValue val) return val;
        
        throw new InvalidOperationException(
            $"Wrong type for {nameof(GetValue)}(), expected {typeof(T).Name}, found {typeof(TValue).Name}");
    }
}