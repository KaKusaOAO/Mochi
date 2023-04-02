using System;
using System.Text;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier.Suggests;

public class Suggestion : IComparable<Suggestion>
{
    private readonly StringRange _range;
    private readonly string _text;
    private readonly IMessage _tooltip;

    public Suggestion(StringRange range, string text, IMessage tooltip = null)
    {
        _range = range;
        _text = text;
        _tooltip = tooltip;
    }

    public StringRange GetRange()
    {
        return _range;
    }

    public string GetText()
    {
        return _text;
    }

    public IMessage GetTooltip()
    {
        return _tooltip;
    }

    public string Apply(string input)
    {
        if (_range.GetStart() == 0 && _range.GetEnd() == input.Length)
        {
            return _text;
        }

        var result = new StringBuilder();
        if (_range.GetStart() > 0)
        {
            result.Append(input.Substring(0, _range.GetStart()));
        }

        result.Append(_text);
        if (_range.GetEnd() < input.Length)
        {
            result.Append(input.Substring(_range.GetEnd()));
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
        if (range.GetStart() < _range.GetStart())
        {
            result.Append(command.Substring(range.GetStart(), _range.GetStart()));
        }

        result.Append(_text);
        if (range.GetEnd() > _range.GetEnd())
        {
            result.Append(command.Substring(_range.GetEnd(), range.GetEnd()));
        }

        return new Suggestion(range, result.ToString(), _tooltip);
    }
}