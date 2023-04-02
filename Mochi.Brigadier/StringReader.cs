using System;
using System.Text;
using Mochi.Brigadier.Exceptions;

namespace Mochi.Brigadier;

public class StringReader : IMutableStringReader
{
    private const char SyntaxEscape = '\\';
    private const char SyntaxDoubleQuote = '"';
    private const char SyntaxSingleQuote = '\'';

    private string _str;
    private int _cursor;

    public StringReader(StringReader other)
    {
        _str = other._str;
        _cursor = other._cursor;
    }

    public StringReader(string str)
    {
        _str = str;
    }

    public string GetString()
    {
        return _str;
    }

    private void InternalSetCursor(int cursor)
    {
        _cursor = cursor;
    }

    public int Cursor
    {
        get => InternalGetCursor();
        set => InternalSetCursor(value);
    }

    public int RemainingLength => _str.Length - _cursor;

    public int TotalLength => _str.Length;

    private int InternalGetCursor() => _cursor;

    public string GetRead()
    {
#if NETCOREAPP
        return _str[.._cursor];
#else
        return _str.Substring(0, _cursor);
#endif
    }

    public string GetRemaining()
    {
#if NETCOREAPP
        return _str[_cursor..];
#else
            return _str.Substring(_cursor);
#endif
    }

    public bool CanRead(int length)
    {
        return _cursor + length <= _str.Length;
    }

    public bool CanRead()
    {
        return CanRead(1);
    }

    public char Peek()
    {
        return _str[_cursor];
    }

    public char Peek(int offset)
    {
        return _str[_cursor + offset];
    }

    public char Read()
    {
        return _str[_cursor++];
    }

    public void Skip()
    {
        _cursor++;
    }

    public static bool IsAllowedNumber(char c)
    {
        return c >= '0' && c <= '9' || c == '.' || c == '-';
    }

    public static bool IsQuotedStringStart(char c)
    {
        return c is SyntaxDoubleQuote or SyntaxSingleQuote;
    }

    public void SkipWhitespace()
    {
        while (CanRead() && char.IsWhiteSpace(Peek()))
        {
            Skip();
        }
    }

    public int ReadInt()
    {
        var start = _cursor;
        while (CanRead() && IsAllowedNumber(Peek()))
        {
            Skip();
        }

        var number = _str.Substring(start, _cursor - start);
        if (string.IsNullOrEmpty(number))
        {
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedInt().CreateWithContext(this);
        }

        try
        {
            return int.Parse(number);
        }
        catch (Exception ex)
        {
            if (!(ex is FormatException || ex is OverflowException)) throw;
            _cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidInt().CreateWithContext(this, number);
        }
    }

    public long ReadLong()
    {
        var start = _cursor;
        while (CanRead() && IsAllowedNumber(Peek()))
        {
            Skip();
        }

        var number = _str.Substring(start, _cursor - start);
        if (string.IsNullOrEmpty(number))
        {
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedLong().CreateWithContext(this);
        }

        try
        {
            return long.Parse(number);
        }
        catch (Exception ex)
        {
            if (!(ex is FormatException || ex is OverflowException)) throw;
            _cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidLong().CreateWithContext(this, number);
        }
    }

    public double ReadDouble()
    {
        var start = _cursor;
        while (CanRead() && IsAllowedNumber(Peek()))
        {
            Skip();
        }

        var number = _str.Substring(start, _cursor - start);
        if (string.IsNullOrEmpty(number))
        {
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedDouble().CreateWithContext(this);
        }

        try
        {
            return double.Parse(number);
        }
        catch (Exception ex)
        {
            if (!(ex is FormatException || ex is OverflowException)) throw;
            _cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidDouble().CreateWithContext(this, number);
        }
    }

    public float ReadFloat()
    {
        var start = _cursor;
        while (CanRead() && IsAllowedNumber(Peek()))
        {
            Skip();
        }

        var number = _str.Substring(start, _cursor - start);
        if (string.IsNullOrEmpty(number))
        {
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedFloat().CreateWithContext(this);
        }

        try
        {
            return float.Parse(number);
        }
        catch (Exception ex)
        {
            if (!(ex is FormatException || ex is OverflowException)) throw;
            _cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidFloat().CreateWithContext(this, number);
        }
    }

    public static bool IsAllowedInUnquotedString(char c)
    {
        return c >= '0' && c <= '9'
               || c >= 'A' && c <= 'Z'
               || c >= 'a' && c <= 'z'
               || c == '_' || c == '-'
               || c == '.' || c == '+';
    }

    public string ReadUnquotedString()
    {
        var start = _cursor;
        while (CanRead() && IsAllowedInUnquotedString(Peek()))
        {
            Skip();
        }

        return _str.Substring(start, _cursor - start);
    }

    public string ReadQuotedString()
    {
        if (!CanRead())
        {
            return "";
        }

        var next = Peek();
        if (!IsQuotedStringStart(next))
        {
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedStartOfQuote().CreateWithContext(this);
        }

        Skip();
        return ReadStringUntil(next);
    }

    public string ReadStringUntil(char terminator)
    {
        var result = new StringBuilder();
        var escaped = false;
        while (CanRead())
        {
            var c = Read();
            if (escaped)
            {
                if (c == terminator || c == SyntaxEscape)
                {
                    result.Append(c);
                    escaped = false;
                }
                else
                {
                    InternalSetCursor(InternalGetCursor() - 1);
                    throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidEscape()
                        .CreateWithContext(this, "" + c);
                }
            }
            else if (c == SyntaxEscape)
            {
                escaped = true;
            }
            else if (c == terminator)
            {
                return result.ToString();
            }
            else
            {
                result.Append(c);
            }
        }

        throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedEndOfQuote().CreateWithContext(this);
    }

    public string ReadString()
    {
        if (!CanRead())
        {
            return "";
        }

        var next = Peek();
        if (IsQuotedStringStart(next))
        {
            Skip();
            return ReadStringUntil(next);
        }

        return ReadUnquotedString();
    }

    public bool ReadBoolean()
    {
        var start = _cursor;
        var value = ReadString();
        if (string.IsNullOrEmpty(value))
        {
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedBool().CreateWithContext(this);
        }

        if (value.Equals("true"))
        {
            return true;
        }

        if (value.Equals("false"))
        {
            return false;
        }

        _cursor = start;
        throw CommandSyntaxException.BuiltInExceptions.ReaderInvalidBool().CreateWithContext(this, value);
    }

    public void Expect(char c)
    {
        if (!CanRead() || Peek() != c)
        {
            throw CommandSyntaxException.BuiltInExceptions.ReaderExpectedSymbol().CreateWithContext(this, "" + c);
        }

        Skip();
    }
}