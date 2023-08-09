namespace Mochi.Irc;

public interface IIrcTagValue
{
    public string RawValue { get; }
}

public class LiteralIrcTagValue : IIrcTagValue
{
    public string Value { get; }
    string IIrcTagValue.RawValue => Value;

    public LiteralIrcTagValue(string value)
    {
        Value = value;
    }
}