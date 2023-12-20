using System;

namespace Mochi.Brigadier.Context;

public struct StringRange
{
    private readonly int _start;
    private readonly int _end;

    public StringRange(int start, int end)
    {
        _start = start;
        _end = end;
    }

    public static StringRange At(int pos) => new(pos, pos);

    public static StringRange Between(int start, int end) => new(start, end);

    public static StringRange Encompassing(StringRange a, StringRange b) =>
        new(Math.Min(a.Start, b.Start), Math.Max(a.End, b.End));

    public static bool operator ==(StringRange a, StringRange b) => a.Equals(b);
    public static bool operator !=(StringRange a, StringRange b) => !(a == b);

    public int Start => _start;

    public int End => _end;

    public string Get(IMutableStringReader reader) => reader.GetString().Substring(_start, Length);

    public string Get(string str) => str.Substring(_start, Length);

    public bool IsEmpty => _start == _end;

    public int Length => _end - _start;

    public override bool Equals(object? o)
    {
        if (o is not StringRange that) return false;
        return _start == that._start && _end == that._end;
    }

    public override int GetHashCode() => HashCode.Combine(_start, _end);

    public override string ToString() => $"StringRange{{start={_start}, end={_end}}}";
}