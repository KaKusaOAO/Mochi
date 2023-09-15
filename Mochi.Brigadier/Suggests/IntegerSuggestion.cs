using System;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier.Suggests;

public class IntegerSuggestion : Suggestion
{
    public int Value { get; }
    
    public IntegerSuggestion(StringRange range, int value, IBrigadierMessage? tooltip = null) : base(range, value.ToString(),
        tooltip)
    {
        Value = value;
    }

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is not IntegerSuggestion that) return false;
        return Value == that.Value && base.Equals(o);
    }

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Value);

    public override string ToString() => 
        $"IntegerSuggestion{{value={Value}, range={Range}, text='{Text}', tooltip='{Tooltip}'}}";

    public override int CompareTo(Suggestion? o)
    {
        if (o is IntegerSuggestion suggestion)
        {
            return Value.CompareTo(suggestion.Value);
        }

        return base.CompareTo(o);
    }

    public override int CompareToIgnoreCase(Suggestion b)
    {
        return CompareTo(b);
    }
}