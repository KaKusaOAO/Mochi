using System;
using System.Text;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier.Suggests;

public class Suggestion : IComparable<Suggestion>
{
    public StringRange Range { get; }
    public string Text { get; }
    public IBrigadierMessage? Tooltip { get; }
    
    public Suggestion(StringRange range, string text, IBrigadierMessage? tooltip = null)
    {
        Range = range;
        Text = text;
        Tooltip = tooltip;
    }

    public string Apply(string input)
    {
        if (Range.Start == 0 && Range.End == input.Length)
        {
            return Text;
        }

        var result = new StringBuilder();
        if (Range.Start > 0)
        {
            result.Append(input.Substring(0, Range.Start));
        }

        result.Append(Text);
        if (Range.End < input.Length)
        {
            result.Append(input.Substring(Range.End));
        }

        return result.ToString();
    }

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is not Suggestion that) return false;
        return Range == that.Range && Text == that.Text && Tooltip == that.Tooltip;
    }

    public override int GetHashCode() => HashCode.Combine(Range, Text, Tooltip);

    public override string ToString() => $"Suggestion{{range={Range}, text='{Text}', tooltip='{Tooltip}'}}";

    public virtual int CompareTo(Suggestion? o) => 
        string.Compare(Text, o?.Text, StringComparison.Ordinal);

    public virtual int CompareToIgnoreCase(Suggestion b) => 
        string.Compare(Text.ToLowerInvariant(), b.Text.ToLowerInvariant(), StringComparison.Ordinal);

    public Suggestion Expand(string command, StringRange range)
    {
        if (range.Equals(Range))
        {
            return this;
        }

        var result = new StringBuilder();
        if (range.Start < Range.Start)
        {
            result.Append(command.Substring(range.Start, Range.Start));
        }

        result.Append(Text);
        if (range.End > Range.End)
        {
            result.Append(command.Substring(Range.End, range.End));
        }

        return new Suggestion(range, result.ToString(), Tooltip);
    }
}