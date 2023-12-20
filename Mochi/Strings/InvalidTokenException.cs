namespace Mochi.Strings;

public class InvalidTokenException : StringParserException
{
    public InvalidTokenException(ReaderTokenType type, string value) : base($"Invalid {type.ToFriendlyName()}: {value}")
    {
        TokenType = type;
        InvalidValue = value;
    }
        
    public ReaderTokenType TokenType { get; }
    public string InvalidValue { get; }
}