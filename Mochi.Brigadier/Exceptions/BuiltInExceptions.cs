namespace Mochi.Brigadier.Exceptions
{
    public class BuiltInExceptions : IBuiltInExceptionProvider {
        private static readonly Dynamic2CommandExceptionType InternalDoubleTooSmall = new Dynamic2CommandExceptionType((found, min) => new LiteralMessage("Double must not be less than " + min + ", found " + found));
        private static readonly Dynamic2CommandExceptionType InternalDoubleTooBig = new Dynamic2CommandExceptionType((found, max) => new LiteralMessage("Double must not be more than " + max + ", found " + found));

        private static readonly Dynamic2CommandExceptionType InternalFloatTooSmall = new Dynamic2CommandExceptionType((found, min) => new LiteralMessage("Float must not be less than " + min + ", found " + found));
        private static readonly Dynamic2CommandExceptionType InternalFloatTooBig = new Dynamic2CommandExceptionType((found, max) => new LiteralMessage("Float must not be more than " + max + ", found " + found));

        private static readonly Dynamic2CommandExceptionType InternalIntegerTooSmall = new Dynamic2CommandExceptionType((found, min) => new LiteralMessage("Integer must not be less than " + min + ", found " + found));
        private static readonly Dynamic2CommandExceptionType InternalIntegerTooBig = new Dynamic2CommandExceptionType((found, max) => new LiteralMessage("Integer must not be more than " + max + ", found " + found));

        private static readonly Dynamic2CommandExceptionType InternalLongTooSmall = new Dynamic2CommandExceptionType((found, min) => new LiteralMessage("Long must not be less than " + min + ", found " + found));
        private static readonly Dynamic2CommandExceptionType InternalLongTooBig = new Dynamic2CommandExceptionType((found, max) => new LiteralMessage("Long must not be more than " + max + ", found " + found));

        private static readonly DynamicCommandExceptionType InternalLiteralIncorrect = new DynamicCommandExceptionType(expected => new LiteralMessage("Expected literal " + expected));

        private static readonly SimpleCommandExceptionType InternalReaderExpectedStartOfQuote = new SimpleCommandExceptionType(new LiteralMessage("Expected quote to start a string"));
        private static readonly SimpleCommandExceptionType InternalReaderExpectedEndOfQuote = new SimpleCommandExceptionType(new LiteralMessage("Unclosed quoted string"));
        private static readonly DynamicCommandExceptionType InternalReaderInvalidEscape = new DynamicCommandExceptionType(character => new LiteralMessage("Invalid escape sequence '" + character + "' in quoted string"));
        private static readonly DynamicCommandExceptionType InternalReaderInvalidBool = new DynamicCommandExceptionType(value => new LiteralMessage("Invalid bool, expected true or false but found '" + value + "'"));
        private static readonly DynamicCommandExceptionType InternalReaderInvalidInt = new DynamicCommandExceptionType(value => new LiteralMessage("Invalid integer '" + value + "'"));
        private static readonly SimpleCommandExceptionType InternalReaderExpectedInt = new SimpleCommandExceptionType(new LiteralMessage("Expected integer"));
        private static readonly DynamicCommandExceptionType InternalReaderInvalidLong = new DynamicCommandExceptionType(value => new LiteralMessage("Invalid long '" + value + "'"));
        private static readonly SimpleCommandExceptionType InternalReaderExpectedLong = new SimpleCommandExceptionType((new LiteralMessage("Expected long")));
        private static readonly DynamicCommandExceptionType InternalReaderInvalidDouble = new DynamicCommandExceptionType(value => new LiteralMessage("Invalid double '" + value + "'"));
        private static readonly SimpleCommandExceptionType InternalReaderExpectedDouble = new SimpleCommandExceptionType(new LiteralMessage("Expected double"));
        private static readonly DynamicCommandExceptionType InternalReaderInvalidFloat = new DynamicCommandExceptionType(value => new LiteralMessage("Invalid float '" + value + "'"));
        private static readonly SimpleCommandExceptionType InternalReaderExpectedFloat = new SimpleCommandExceptionType(new LiteralMessage("Expected float"));
        private static readonly SimpleCommandExceptionType InternalReaderExpectedBool = new SimpleCommandExceptionType(new LiteralMessage("Expected bool"));
        private static readonly DynamicCommandExceptionType InternalReaderExpectedSymbol = new DynamicCommandExceptionType(symbol => new LiteralMessage("Expected '" + symbol + "'"));

        private static readonly SimpleCommandExceptionType InternalDispatcherUnknownCommand = new SimpleCommandExceptionType(new LiteralMessage("Unknown command"));
        private static readonly SimpleCommandExceptionType InternalDispatcherUnknownArgument = new SimpleCommandExceptionType(new LiteralMessage("Incorrect argument for command"));
        private static readonly SimpleCommandExceptionType InternalDispatcherExpectedArgumentSeparator = new SimpleCommandExceptionType(new LiteralMessage("Expected whitespace to end one argument, but found trailing data"));
        private static readonly DynamicCommandExceptionType InternalDispatcherParseException = new DynamicCommandExceptionType(message => new LiteralMessage("Could not parse command: " + message));

        public Dynamic2CommandExceptionType DoubleTooLow() {
            return InternalDoubleTooSmall;
        }

        public Dynamic2CommandExceptionType DoubleTooHigh() {
            return InternalDoubleTooBig;
        }

        public Dynamic2CommandExceptionType FloatTooLow() {
            return InternalFloatTooSmall;
        }

        public Dynamic2CommandExceptionType FloatTooHigh() {
            return InternalFloatTooBig;
        }

    
        public Dynamic2CommandExceptionType IntegerTooLow() {
            return InternalIntegerTooSmall;
        }

    
        public Dynamic2CommandExceptionType IntegerTooHigh() {
            return InternalIntegerTooBig;
        }

    
        public Dynamic2CommandExceptionType LongTooLow() {
            return InternalLongTooSmall;
        }

    
        public Dynamic2CommandExceptionType LongTooHigh() {
            return InternalLongTooBig;
        }

    
        public DynamicCommandExceptionType LiteralIncorrect() {
            return InternalLiteralIncorrect;
        }

    
        public SimpleCommandExceptionType ReaderExpectedStartOfQuote() {
            return InternalReaderExpectedStartOfQuote;
        }

    
        public SimpleCommandExceptionType ReaderExpectedEndOfQuote() {
            return InternalReaderExpectedEndOfQuote;
        }

    
        public DynamicCommandExceptionType ReaderInvalidEscape() {
            return InternalReaderInvalidEscape;
        }

    
        public DynamicCommandExceptionType ReaderInvalidBool() {
            return InternalReaderInvalidBool;
        }

    
        public DynamicCommandExceptionType ReaderInvalidInt() {
            return InternalReaderInvalidInt;
        }

    
        public SimpleCommandExceptionType ReaderExpectedInt() {
            return InternalReaderExpectedInt;
        }

    
        public DynamicCommandExceptionType ReaderInvalidLong() {
            return InternalReaderInvalidLong;
        }

    
        public SimpleCommandExceptionType ReaderExpectedLong() {
            return InternalReaderExpectedLong;
        }

    
        public DynamicCommandExceptionType ReaderInvalidDouble() {
            return InternalReaderInvalidDouble;
        }

    
        public SimpleCommandExceptionType ReaderExpectedDouble() {
            return InternalReaderExpectedDouble;
        }

    
        public DynamicCommandExceptionType ReaderInvalidFloat() {
            return InternalReaderInvalidFloat;
        }

    
        public SimpleCommandExceptionType ReaderExpectedFloat() {
            return InternalReaderExpectedFloat;
        }

    
        public SimpleCommandExceptionType ReaderExpectedBool() {
            return InternalReaderExpectedBool;
        }

    
        public DynamicCommandExceptionType ReaderExpectedSymbol() {
            return InternalReaderExpectedSymbol;
        }

    
        public SimpleCommandExceptionType DispatcherUnknownCommand() {
            return InternalDispatcherUnknownCommand;
        }

    
        public SimpleCommandExceptionType DispatcherUnknownArgument() {
            return InternalDispatcherUnknownArgument;
        }

    
        public SimpleCommandExceptionType DispatcherExpectedArgumentSeparator() {
            return InternalDispatcherExpectedArgumentSeparator;
        }

    
        public DynamicCommandExceptionType DispatcherParseException() {
            return InternalDispatcherParseException;
        }
    }
}