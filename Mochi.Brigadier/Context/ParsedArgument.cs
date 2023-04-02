﻿using System;

namespace Mochi.Brigadier.Context;

public class ParsedArgument<TS>
{
    private readonly StringRange _range;
    private readonly object _result;

    public ParsedArgument(int start, int end, object result) {
        this._range = StringRange.Between(start, end);
        this._result = result;
    }

    public StringRange GetRange() {
        return _range;
    }

    public object GetResult() {
        return _result;
    }

    public override bool Equals(object o) {
        if (this == o) {
            return true;
        }
        if (!(o is ParsedArgument<TS>)) {
            return false;
        }
        
        var that = o as ParsedArgument<TS>;
        return _result == that?._result && _range == that._range;
    }

    public override int GetHashCode() {
#if NETCOREAPP
            return HashCode.Combine(_range, _result);
#else
        return _range.GetHashCode() * 31 + _result.GetHashCode();
#endif
    }
}

public class ParsedArgument<TS, T> : ParsedArgument<TS> {
    public ParsedArgument(int start, int end, T result) : base(start, end, result)
    {
    }
    
    public new T GetResult() {
        return (T)base.GetResult();
    }
}