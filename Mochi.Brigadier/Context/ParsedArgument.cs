using System;

namespace Mochi.Brigadier.Context;

public interface IParsedArgument<T>
{
    public object Result { get; }
    public StringRange Range { get; }
}

public interface IParsedArgument<TSource, out T> : IParsedArgument<TSource>
{
    public new T Result { get; }
    object IParsedArgument<TSource>.Result => Result!;
}

public class ParsedArgument<TSource, T> : IParsedArgument<TSource, T>
{
    public StringRange Range { get; }

    public T Result { get; }
    
    public ParsedArgument(int start, int end, T result)
    {
        Range = StringRange.Between(start, end);
        Result = result;
    }

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is not ParsedArgument<TSource, T> that) return false;
        return Equals(Result, that.Result) && Range == that.Range;
    }

    public override int GetHashCode() => HashCode.Combine(Range, Result);
}