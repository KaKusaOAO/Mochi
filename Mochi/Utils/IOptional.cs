using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mochi.Utils;

public interface IOptional
{
    public bool IsPresent { get; }
    public bool IsEmpty { get; }
    public object Value { get; }
}

public interface IOptional<out T> : IOptional
{
    public new T Value { get; }
    object IOptional.Value => Value!;
}

public static class Optional
{
    public static IOptional<T> Of<T>(T value) => new Optional<T>(value);
    public static IOptional<T> Empty<T>() => new Optional<T>();

    public static IOptional<T> OfNullable<T>(T? value) where T : class =>
        value == null ? new Optional<T>() : new Optional<T>(value);

    public static IOptional<T> OfNullable<T>(T? value) where T : struct =>
        value.HasValue ? new Optional<T>(value.Value) : new Optional<T>();

    // -- Extension

    public static IOptional<T> Select<T>(this IOptional optional, Func<object, T> transform) => 
        optional.IsEmpty ? Empty<T>() : new Optional<T>(transform(optional.Value));

    public static IOptional<TOut> Select<TIn, TOut>(this IOptional<TIn> optional, Func<TIn, TOut> transform) => 
        optional.IsEmpty ? Empty<TOut>() : new Optional<TOut>(transform(optional.Value));

    public static IOptional<T> OfType<T>(this IOptional optional)
    {
        if (optional.IsEmpty) return Empty<T>();
        try
        {
            var val = (T)optional.Value;
            return new Optional<T>(val);
        }
        catch
        {
            return new Optional<T>();
        } 
    }

    public static IOptional<T> IfPresent<T>(this IOptional<T> optional, Action<T> action)
    {
        if (optional.IsPresent)
        {
            action(optional.Value);
        }

        return optional;
    }

    public static IOptional<T> IfEmpty<T>(this IOptional<T> optional, Action action)
    {
        if (optional.IsEmpty) action();
        return optional;
    }

    public static T OrElse<T>(this IOptional<T> optional, T def)
    {
        return optional.IsPresent ? optional.Value : def;
    }
    
    public static T OrElse<T>(this IOptional<T> optional, Func<T> def)
    {
        return optional.IsPresent ? optional.Value : def();
    }

    public static IOptional<T> Or<T>(this IOptional<T> optional, Func<IOptional<T>> other)
    {
        return optional.IsPresent ? optional : other();
    }

    public static IOptional<T> Or<T>(this IOptional<T> optional, IOptional<T> other) => optional.Or(() => other);

    public static IEnumerable<T> WherePresent<T>(this IEnumerable<IOptional<T>> enumerable)
    {
        return from optional in enumerable
            where optional.IsPresent
            select optional.Value;
    }

    public static IEnumerable<TOut> SelectOptional<TIn, TOut>(this IEnumerable<IOptional<TIn>> enumerable, Func<TIn, TOut> transform)
    {
        return from optional in enumerable
            where optional.IsPresent
            select transform(optional.Value);
    }
    
    public static IEnumerable<T> SelectOptional<T>(this IEnumerable<IOptional<T>> enumerable) => 
        SelectOptional(enumerable, t => t);
    
    public static IEnumerable<T> WhereOptional<T>(this IEnumerable<IOptional<T>> enumerable, Func<T, bool> filter)
    {
        return from v in SelectOptional(enumerable)
            where filter(v) 
            select v;
    }
}

internal readonly struct Optional<T> : IOptional<T>
{
    private readonly bool _hasValue;
    private readonly T? _value;

    public Optional()
    {
        
    }

    public Optional(T value)
    {
        if (value == null) throw new ArgumentException("Value not present");
        _hasValue = true;
        _value = value;
    }

    public bool IsPresent => _hasValue;
    public bool IsEmpty => !_hasValue;

    public T Value => _hasValue ? _value! : throw new InvalidOperationException("No value present");
}