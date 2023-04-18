using System;

namespace Mochi.Brigadier.Context;

public class ParsedArgument<TS>
{
    private readonly StringRange _range;
    private readonly object _result;

    public ParsedArgument(int start, int end, object result)
    {
        _range = StringRange.Between(start, end);
        _result = result;
    }

    public StringRange Range => _range;

    public object Result => _result;

    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (o is not ParsedArgument<TS> that) return false;
        
        return _result == that._result && _range == that._range;
    }

    public override int GetHashCode() => HashCode.Combine(_range, _result);
}

public class ParsedArgument<TS, T> : ParsedArgument<TS>
{
    public ParsedArgument(int start, int end, T result) : base(start, end, result)
    {
    }

    public new T Result => (T) base.Result;
}