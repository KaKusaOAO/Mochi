using System;

namespace Mochi.Brigadier.Context;

public struct StringRange {
    private readonly int _start;
    private readonly int _end;

    public StringRange(int start, int end) {
        this._start = start;
        this._end = end;
    }

    public static StringRange At(int pos) {
        return new StringRange(pos, pos);
    }

    public static StringRange Between(int start, int end) {
        return new StringRange(start, end);
    }

    public static StringRange Encompassing(StringRange a, StringRange b) {
        return new StringRange(Math.Min(a.GetStart(), b.GetStart()), Math.Max(a.GetEnd(), b.GetEnd()));
    }

    public static bool operator ==(StringRange a, StringRange b) => a.Equals(b);

    public static bool operator !=(StringRange a, StringRange b) => !(a == b);

    public int GetStart() {
        return _start;
    }

    public int GetEnd() {
        return _end;
    }

    public string Get(IMutableStringReader reader) {
        return reader.GetString().Substring(_start, GetLength());
    }

    public string Get(string str) {
        return str.Substring(_start, GetLength());
    }

    public bool IsEmpty() {
        return _start == _end;
    }

    public int GetLength() {
        return _end - _start;
    }

    public override bool Equals(object o)
    {
        if (!(o is StringRange that)) {
            return false;
        }
        
        return _start == that._start && _end == that._end;
    }

    public override int GetHashCode() 
    {
#if NETCOREAPP
            return HashCode.Combine(_start, _end);
#else
        return _start * 31 + _end;
#endif
    }

    public override string ToString()
    {
        return $"StringRange{{start={_start}, end={_end}}}";
    }
}