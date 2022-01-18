using System.Text;
using KaLib.Brigadier.Context;

namespace KaLib.Brigadier.Suggests;

public class Suggestion : IComparable<Suggestion> {
    private readonly StringRange _range;
    private readonly string _text;
    private readonly IMessage? _tooltip;
    public Suggestion(StringRange range, string text, IMessage? tooltip = null) {
        this._range = range;
        this._text = text;
        this._tooltip = tooltip;
    }

    public StringRange GetRange() {
        return _range;
    }

    public string GetText() {
        return _text;
    }

    public IMessage GetTooltip() {
        return _tooltip;
    }

    public string Apply(string input) {
        if (_range.GetStart() == 0 && _range.GetEnd() == input.Length) {
            return _text;
        }
        var result = new StringBuilder();
        if (_range.GetStart() > 0) {
            result.Append(input[.._range.GetStart()]);
        }
        result.Append(_text);
        if (_range.GetEnd() < input.Length) {
            result.Append(input[_range.GetEnd()..]);
        }
        return result.ToString();
    }

    public override bool Equals(object o) {
        if (this == o) {
            return true;
        }
        if (!(o is Suggestion)) {
            return false;
        }
        var that = (Suggestion) o;
        return _range == that._range && _text == that._text && _tooltip == that._tooltip;
    }

    public override int GetHashCode() {
        return HashCode.Combine(_range, _text, _tooltip);
    }

    public override string ToString() {
        return $"Suggestion{{range={_range}, text='{_text}', tooltip='{_tooltip}'}}";
    }

    public int CompareTo(Suggestion? o) {
        return string.Compare(_text, o?._text, StringComparison.Ordinal);
    }

    public int CompareToIgnoreCase(Suggestion b) {
        return string.Compare(_text.ToLowerInvariant(), b._text.ToLowerInvariant(), StringComparison.Ordinal);
    }

    public Suggestion Expand(string command, StringRange range) {
        if (range.Equals(this._range)) {
            return this;
        }
        var result = new StringBuilder();
        if (range.GetStart() < this._range.GetStart()) {
            result.Append(command.Substring(range.GetStart(), this._range.GetStart()));
        }
        result.Append(_text);
        if (range.GetEnd() > this._range.GetEnd()) {
            result.Append(command.Substring(this._range.GetEnd(), range.GetEnd()));
        }
        return new Suggestion(range, result.ToString(), _tooltip);
    }
}
