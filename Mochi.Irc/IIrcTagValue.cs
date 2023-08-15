namespace Mochi.Irc;

public interface IIrcTagValue
{
    public string RawValue { get; }
}

public class LiteralIrcTagValue : IIrcTagValue
{
    public string Value { get; }
    string IIrcTagValue.RawValue => Value;

    public static implicit operator LiteralIrcTagValue(string value) => new LiteralIrcTagValue(value);

    public LiteralIrcTagValue(string value)
    {
        Value = value;
    }
}