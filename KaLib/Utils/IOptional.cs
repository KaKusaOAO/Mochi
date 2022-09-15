using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaLib.Utils;

public interface IOptional
{
    public bool IsPresent { get; }
    public bool IsEmpty { get; }
}

public interface IOptional<out T> : IOptional
{
    public T Value { get; }
}

public static class Optional
{
    public static IOptional<T> Of<T>(T value) => new Optional<T>(value);
    public static IOptional<T> Empty<T>() => new Optional<T>();

    public static IOptional<T> OfNullable<T>(T value) where T : class =>
        value == null ? new Optional<T>() : new Optional<T>(value);

    public static IOptional<T> OfNullable<T>(T? value) where T : struct =>
        value.HasValue ? new Optional<T>(value.Value) : new Optional<T>();
    
    // -- Extension

    public static IOptional<T> AsOptional<T>(T? value) where T : struct => OfNullable(value);

    public static IOptional<TOut> Select<TIn, TOut>(this IOptional<TIn> optional, Func<TIn, TOut> transform)
    {
        return optional.IsEmpty ? Empty<TOut>() : new Optional<TOut>(transform(optional.Value));
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

    // -- Async extension
    public static async Task<IOptional<T>> IfPresentAsync<T>(this IOptional<T> optional, Func<T, Task> action)
    {
        if (optional.IsPresent)
        {
            await action(optional.Value);
        }

        return optional;
    }
    
    public static async Task<IOptional<T>> IfPresentAsync<T>(this Task<IOptional<T>> optional, Func<T, Task> action)
    {
        var o = await optional;
        if (o.IsPresent)
        {
            await action(o.Value);
        }

        return o;
    }

    public static Task<IOptional<T>> IfPresentAsync<T>(this Task<IOptional<T>> optional, Action<T> action)
    {
        return optional.IfPresentAsync(async v =>
        {
            await Task.Yield();
            action(v);
        });
    }
    
    public static async Task<IOptional<T>> IfEmptyAsync<T>(this IOptional<T> optional, Func<T, Task> action)
    {
        if (optional.IsEmpty)
        {
            await action(optional.Value);
        }

        return optional;
    }
    
    public static async Task<IOptional<T>> IfEmptyAsync<T>(this Task<IOptional<T>> optional, Func<T, Task> action)
    {
        var o = await optional;
        if (o.IsEmpty)
        {
            await action(o.Value);
        }

        return o;
    }

    public static Task<IOptional<T>> IfEmptyAsync<T>(this Task<IOptional<T>> optional, Action<T> action)
    {
        return optional.IfEmptyAsync(async v =>
        {
            await Task.Yield();
            action(v);
        });
    }

    public static async Task<IOptional<TOut>> SelectAsync<TIn, TOut>(this IOptional<TIn> optional,
        Func<TIn, Task<TOut>> transform)
    {
        return optional.IsPresent ? new Optional<TOut>(await transform(optional.Value)) : Empty<TOut>();
    }

    public static async Task<IOptional<TOut>> SelectAsync<TIn, TOut>(this Task<IOptional<TIn>> optional,
        Func<TIn, Task<TOut>> transform)
    {
        var o = await optional;
        return o.IsPresent ? new Optional<TOut>(await transform(o.Value)) : Empty<TOut>();
    }
    
    public static Task<IOptional<TOut>> SelectAsync<TIn, TOut>(this Task<IOptional<TIn>> optional,
        Func<TIn, TOut> transform)
    {
        return optional.SelectAsync(async v =>
        {
            await Task.Yield();
            return transform(v);
        });
    }

    public static async Task<T> OrElseAsync<T>(this Task<IOptional<T>> optional, T def)
    {
        var o = await optional;
        return o.OrElse(def);
    }
    
    public static async Task<T> OrElseAsync<T>(this Task<IOptional<T>> optional, Func<T> def)
    {
        var o = await optional;
        return o.OrElse(def);
    }

    public static async Task<T> OrElseAsync<T>(this IOptional<T> optional, Func<Task<T>> def)
    {
        return optional.IsPresent ? optional.Value : await def();
    }
}

internal class Optional<T> : IOptional<T>
{
    private readonly bool _hasValue;
    private readonly T _value;

    public Optional()
    {
    }

    public Optional(T value)
    {
        _value = value;
        _hasValue = true;
    }

    public bool IsPresent => _hasValue;
    public bool IsEmpty => !_hasValue;

    public T Value => _hasValue ? _value : throw new InvalidOperationException("No value present");
}