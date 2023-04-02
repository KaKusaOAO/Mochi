using System;
using System.Text;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier.Suggests;

public class Suggestion : IComparable<Suggestion>
{
    private readonly StringRange _range;
    private readonly string _text;
    private readonly IBrigadierMessage _tooltip;

    public Suggestion(StringRange range, string text, IBrigadierMessage tooltip = null)
    {
        _range = range;
        _text = text;
        _tooltip = tooltip;
    }

    public StringRange Range => _range;

    public string Text => _text;

    public IBrigadierMessage Tooltip => _tooltip;

    public string Apply(string input)
    {
        if (_range.Start == 0 && _range.End == input.Length)
        {
            return _text;
        }

        var result = new StringBuilder();
        if (_range.Start > 0)
        {
            result.Append(input.Substring(0, _range.Start));
        }

        result.Append(_text);
        if (_range.End < input.Length)
        {
            result.Append(input.Substring(_range.End));
        }

        return result.ToString();
    }

    public override bool Equals(object o)
    {
        if (this == o)
        {
            return true;
        }

        if (!(o is Suggestion))
        {
            return false;
        }

        var that = (Suggestion)o;
        return _range == that._range && _text == that._text && _tooltip == that._tooltip;
    }

    public override int GetHashCode()
    {
#if NETCOREAPP
            return HashCode.Combine(_range, _text, _tooltip);
#else
        return (_range.GetHashCode() * 31 + _text.GetHashCode()) * 31 + _tooltip.GetHashCode();
#endif
    }

    public override string ToString()
    {
        return $"Suggestion{{range={_range}, text='{_text}', tooltip='{_tooltip}'}}";
    }

    public virtual int CompareTo(Suggestion o)
    {
        return string.Compare(_text, o?._text, StringComparison.Ordinal);
    }

    public virtual int CompareToIgnoreCase(Suggestion b)
    {
        return string.Compare(_text.ToLowerInvariant(), b._text.ToLowerInvariant(), StringComparison.Ordinal);
    }

    public Suggestion Expand(string command, StringRange range)
    {
        if (range.Equals(_range))
        {
            return this;
        }

        var result = new StringBuilder();
        if (range.Start < _range.Start)
        {
            result.Append(command.Substring(range.Start, _range.Start));
        }

        result.Append(_text);
        if (range.End > _range.End)
        {
            result.Append(command.Substring(_range.End, range.End));
        }

        return new Suggestion(range, result.ToString(), _tooltip);
    }
}