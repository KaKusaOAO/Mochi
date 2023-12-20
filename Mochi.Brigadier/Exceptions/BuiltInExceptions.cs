namespace Mochi.Brigadier.Exceptions;

public class BuiltInExceptions : IBuiltInExceptionProvider
{
    #region --> Internal implementations
    private static readonly Dynamic2CommandExceptionType _internalDoubleTooSmall =
        new((found, min) =>
            new LiteralMessage("Double must not be less than " + min + ", found " + found));

    private static readonly Dynamic2CommandExceptionType _internalDoubleTooBig =
        new((found, max) =>
            new LiteralMessage("Double must not be more than " + max + ", found " + found));

    private static readonly Dynamic2CommandExceptionType _internalFloatTooSmall =
        new((found, min) =>
            new LiteralMessage("Float must not be less than " + min + ", found " + found));

    private static readonly Dynamic2CommandExceptionType _internalFloatTooBig =
        new((found, max) =>
            new LiteralMessage("Float must not be more than " + max + ", found " + found));

    private static readonly Dynamic2CommandExceptionType _internalIntegerTooSmall =
        new((found, min) =>
            new LiteralMessage("Integer must not be less than " + min + ", found " + found));

    private static readonly Dynamic2CommandExceptionType _internalIntegerTooBig =
        new((found, max) =>
            new LiteralMessage("Integer must not be more than " + max + ", found " + found));

    private static readonly Dynamic2CommandExceptionType _internalLongTooSmall =
        new((found, min) =>
            new LiteralMessage("Long must not be less than " + min + ", found " + found));

    private static readonly Dynamic2CommandExceptionType _internalLongTooBig =
        new((found, max) =>
            new LiteralMessage("Long must not be more than " + max + ", found " + found));

    private static readonly DynamicCommandExceptionType _internalLiteralIncorrect =
        new(expected => new LiteralMessage("Expected literal " + expected));

    private static readonly SimpleCommandExceptionType _internalReaderExpectedStartOfQuote =
        new(new LiteralMessage("Expected quote to start a string"));

    private static readonly SimpleCommandExceptionType _internalReaderExpectedEndOfQuote =
        new(new LiteralMessage("Unclosed quoted string"));

    private static readonly DynamicCommandExceptionType _internalReaderInvalidEscape =
        new(character =>
            new LiteralMessage("Invalid escape sequence '" + character + "' in quoted string"));

    private static readonly DynamicCommandExceptionType _internalReaderInvalidBool =
        new(value =>
            new LiteralMessage("Invalid bool, expected true or false but found '" + value + "'"));

    private static readonly DynamicCommandExceptionType _internalReaderInvalidInt =
        new(value => new LiteralMessage("Invalid integer '" + value + "'"));

    private static readonly SimpleCommandExceptionType _internalReaderExpectedInt =
        new(new LiteralMessage("Expected integer"));

    private static readonly DynamicCommandExceptionType _internalReaderInvalidLong =
        new(value => new LiteralMessage("Invalid long '" + value + "'"));

    private static readonly SimpleCommandExceptionType _internalReaderExpectedLong =
        new((new LiteralMessage("Expected long")));

    private static readonly DynamicCommandExceptionType _internalReaderInvalidDouble =
        new(value => new LiteralMessage("Invalid double '" + value + "'"));

    private static readonly SimpleCommandExceptionType _internalReaderExpectedDouble =
        new(new LiteralMessage("Expected double"));

    private static readonly DynamicCommandExceptionType _internalReaderInvalidFloat =
        new(value => new LiteralMessage("Invalid float '" + value + "'"));

    private static readonly SimpleCommandExceptionType _internalReaderExpectedFloat =
        new(new LiteralMessage("Expected float"));

    private static readonly SimpleCommandExceptionType _internalReaderExpectedBool =
        new(new LiteralMessage("Expected bool"));

    private static readonly DynamicCommandExceptionType _internalReaderExpectedSymbol =
        new(symbol => new LiteralMessage("Expected '" + symbol + "'"));

    private static readonly SimpleCommandExceptionType _internalDispatcherUnknownCommand =
        new(new LiteralMessage("Unknown command"));

    private static readonly SimpleCommandExceptionType _internalDispatcherUnknownArgument =
        new(new LiteralMessage("Incorrect argument for command"));

    private static readonly SimpleCommandExceptionType _internalDispatcherExpectedArgumentSeparator =
        new(new LiteralMessage("Expected whitespace to end one argument, but found trailing data"));

    private static readonly DynamicCommandExceptionType _internalDispatcherParseException =
        new(message => new LiteralMessage("Could not parse command: " + message));
    #endregion

    #region --> Interface implementations
    public Dynamic2CommandExceptionType DoubleTooLow() => _internalDoubleTooSmall;
    public Dynamic2CommandExceptionType DoubleTooHigh() => _internalDoubleTooBig;
    public Dynamic2CommandExceptionType FloatTooLow() => _internalFloatTooSmall;
    public Dynamic2CommandExceptionType FloatTooHigh() => _internalFloatTooBig;
    public Dynamic2CommandExceptionType IntegerTooLow() => _internalIntegerTooSmall;
    public Dynamic2CommandExceptionType IntegerTooHigh() => _internalIntegerTooBig;
    public Dynamic2CommandExceptionType LongTooLow() => _internalLongTooSmall;
    public Dynamic2CommandExceptionType LongTooHigh() => _internalLongTooBig;
    public DynamicCommandExceptionType LiteralIncorrect() => _internalLiteralIncorrect;
    public SimpleCommandExceptionType ReaderExpectedStartOfQuote() => _internalReaderExpectedStartOfQuote;
    public SimpleCommandExceptionType ReaderExpectedEndOfQuote() => _internalReaderExpectedEndOfQuote;
    public DynamicCommandExceptionType ReaderInvalidEscape() => _internalReaderInvalidEscape;
    public DynamicCommandExceptionType ReaderInvalidBool() => _internalReaderInvalidBool;
    public DynamicCommandExceptionType ReaderInvalidInt() => _internalReaderInvalidInt;
    public SimpleCommandExceptionType ReaderExpectedInt() => _internalReaderExpectedInt;
    public DynamicCommandExceptionType ReaderInvalidLong() => _internalReaderInvalidLong;
    public SimpleCommandExceptionType ReaderExpectedLong() => _internalReaderExpectedLong;
    public DynamicCommandExceptionType ReaderInvalidDouble() => _internalReaderInvalidDouble;
    public SimpleCommandExceptionType ReaderExpectedDouble() => _internalReaderExpectedDouble;
    public DynamicCommandExceptionType ReaderInvalidFloat() => _internalReaderInvalidFloat;
    public SimpleCommandExceptionType ReaderExpectedFloat() => _internalReaderExpectedFloat;
    public SimpleCommandExceptionType ReaderExpectedBool() => _internalReaderExpectedBool;
    public DynamicCommandExceptionType ReaderExpectedSymbol() => _internalReaderExpectedSymbol;
    public SimpleCommandExceptionType DispatcherUnknownCommand() => _internalDispatcherUnknownCommand;
    public SimpleCommandExceptionType DispatcherUnknownArgument() => _internalDispatcherUnknownArgument;

    public SimpleCommandExceptionType DispatcherExpectedArgumentSeparator() =>
        _internalDispatcherExpectedArgumentSeparator;

    public DynamicCommandExceptionType DispatcherParseException() => _internalDispatcherParseException;
    #endregion
}