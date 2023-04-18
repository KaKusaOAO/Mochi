using System;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier.Suggests;

public class IntegerSuggestion : Suggestion
{
    private readonly int _value;

    public IntegerSuggestion(StringRange range, int value, IBrigadierMessage? tooltip = null) : base(range, value.ToString(),
        tooltip)
    {
        _value = value;
    }

    public int Value => _value;

    public override bool Equals(object o)
    {
        if (this == o)
        {
            return true;
        }

        if (o is not IntegerSuggestion that)
        {
            return false;
        }

        return _value == that._value && base.Equals(o);
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), _value);

    public override string ToString() => 
        $"IntegerSuggestion{{value={_value}, range={Range}, text='{Text}', tooltip='{Tooltip}'}}";

    public override int CompareTo(Suggestion o)
    {
        if (o is IntegerSuggestion suggestion)
        {
            return _value.CompareTo(suggestion._value);
        }

        return base.CompareTo(o);
    }

    public override int CompareToIgnoreCase(Suggestion b)
    {
        return CompareTo(b);
    }
}