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
    public IOptional<T> Where(Predicate<T> predicate);
}

public static class Optional
{
    public static IOptional<T> Of<T>(T value) => new Some<T>(value);
    public static IOptional<T> Empty<T>() => new None<T>();
    
    private readonly struct Some<T> : IOptional<T>
    {
        private readonly T _value;

        public Some(T value)
        {
            _value = value;
        }

        bool IOptional.IsPresent => true;
        bool IOptional.IsEmpty => false;
        T IOptional<T>.Value => _value;
        public IOptional<T> Where(Predicate<T> predicate) => predicate(_value) ? this : new None<T>();
    }

    private readonly struct None<T> : IOptional<T>
    {
        bool IOptional.IsPresent => false;
        bool IOptional.IsEmpty => true;
        T IOptional<T>.Value => throw new InvalidOperationException("No value present");
        public IOptional<T> Where(Predicate<T> predicate) => this;
    }

    public static IOptional<T> OfNullable<T>(T? value) where T : class =>
        value == null ? Empty<T>() : Of(value);

    public static IOptional<T> OfNullable<T>(T? value) where T : struct =>
        value.HasValue ? Of(value.Value) : Empty<T>();

    // -- Extension

    public static IOptional<T> Select<T>(this IOptional optional, Func<object, T> transform) => 
        optional.IsEmpty ? Empty<T>() : Of(transform(optional.Value));

    public static IOptional<TOut> Select<TIn, TOut>(this IOptional<TIn> optional, Func<TIn, TOut> transform) => 
        optional.IsEmpty ? Empty<TOut>() : Of(transform(optional.Value));

    public static IOptional<T> OfType<T>(this IOptional optional)
    {
        if (optional.IsEmpty) return Empty<T>();
        try
        {
            var val = (T)optional.Value;
            return Of(val);
        }
        catch
        {
            return Empty<T>();
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
    
    public static T OrElseGet<T>(this IOptional<T> optional, Func<T> def)
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