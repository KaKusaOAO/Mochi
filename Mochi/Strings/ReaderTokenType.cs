using System;

namespace Mochi.Strings;

public enum ReaderTokenType
{
    Boolean,
    StartOfQuote,
    EndOfQuote,
    Float,
    Double,
    Long,
    Integer,
    Escape,
    ConstantToken
}

public static class ReaderTokenTypeExtension
{
    public static string ToFriendlyName(this ReaderTokenType type)
    {
        return type switch
        {
            ReaderTokenType.Boolean => "boolean",
            ReaderTokenType.StartOfQuote => "start of quote",
            ReaderTokenType.EndOfQuote => "end of quote",
            ReaderTokenType.Float => "float",
            ReaderTokenType.Double => "double",
            ReaderTokenType.Long => "long",
            ReaderTokenType.Integer => "integer",
            ReaderTokenType.Escape => "escape",
            ReaderTokenType.ConstantToken => "constant token",
            _ => throw new IndexOutOfRangeException($"Unexpected token type: {type}")
        };
    }
}