using System;

namespace Mochi.Utils;

public interface IHolder : IOptional
{
    
}

public interface IHolder<T> : IHolder, IOptional<T>
{
    IHolder<T> Remove();
    IHolder<T> Replace(T value);
    IHolder<T> ReplaceNullable(T? value);
}

public static class Holder
{
    public static IHolder<T> Of<T>(T value) => new Holder<T>(value);
    public static IHolder<T> Empty<T>() => new Holder<T>();

    public static IHolder<T> OfNullable<T>(T? value) where T : class =>
        value == null ? new Holder<T>() : new Holder<T>(value);

    public static IHolder<T> OfNullable<T>(T? value) where T : struct =>
        value.HasValue ? new Holder<T>(value.Value) : new Holder<T>();

    public static IHolder<T> ReplaceNullable<T>(this IHolder<T> holder, T? value) where T : struct =>
        value.HasValue ? holder.Replace(value.Value) : holder.Remove();

    public static IHolder<TOut> Select<TIn, TOut>(this IHolder<TIn> optional, Func<TIn, TOut> transform) => 
        optional.IsEmpty ? Empty<TOut>() : new Holder<TOut>(transform(optional.Value));
    
    public static IHolder<T> IfPresent<T>(this IHolder<T> optional, Action<T> action)
    {
        if (optional.IsPresent)
        {
            action(optional.Value);
        }

        return optional;
    }

    public static IHolder<T> IfEmpty<T>(this IHolder<T> optional, Action action)
    {
        if (optional.IsEmpty) action();
        return optional;
    }
    public static IHolder<T> Or<T>(this IHolder<T> optional, Func<IHolder<T>> other)
    {
        return optional.IsPresent ? optional : other();
    }

    public static IHolder<T> Or<T>(this IHolder<T> optional, IHolder<T> other) => optional.Or(() => other);
    public static IHolder<T> AsHolder<T>(this IOptional<T> optional) => new Holder<T>(optional);
}

public class Holder<T> : IHolder<T>
{
    private bool _hasValue;
    private T? _value;
    
    public Holder()
    {
        
    }

    public Holder(T value)
    {
        if (value == null) throw new ArgumentException("Value not present");
        _hasValue = true;
        _value = value;
    }

    public Holder(IOptional<T> optional)
    {
        lock (optional)
        {
            _hasValue = optional.IsPresent;
            if (_hasValue) _value = optional.Value;
        }
    }

    public bool IsPresent => _hasValue;
    public bool IsEmpty => !_hasValue;

    public T Value => _hasValue ? _value! : throw new InvalidOperationException("No value present");
    
    public IOptional<T> Where(Predicate<T> predicate)
    {
        if (!_hasValue) return this;
        return predicate(_value!) ? this : Holder.Empty<T>();
    }

    public IHolder<T> Remove()
    {
        _hasValue = false;
        return this;
    }

    public IHolder<T> Replace(T value)
    {
        if (value == null) throw new ArgumentException("New value not present");
        _hasValue = true;
        _value = value;
        return this;
    }

    public IHolder<T> ReplaceNullable(T? value)
    {
        _hasValue = value != null;
        _value = value;
        return this;
    }
}