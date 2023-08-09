using System;
using System.Text;

namespace Mochi.Strings;

public class StringParser
{
    private const char SyntaxEscape = '\\';
    private const char SyntaxDoubleQuote = '"';
    private const char SyntaxSingleQuote = '\'';

    private readonly string _str;
    private int _cursor;

    public StringParser(StringParser other)
    {
        _str = other._str;
        _cursor = other._cursor;
    }

    public StringParser(string str)
    {
        _str = str;
    }

    public string GetString()
    {
        return _str;
    }
    
    public int RemainingLength => _str.Length - _cursor;

    public int TotalLength => _str.Length;

    public int Cursor
    {
        get => _cursor;
        set => _cursor = value;
    }

    public string GetRead()
    {
#if NETCOREAPP
            return _str[.._cursor];
#else
        return _str.Substring(0, _cursor);
#endif
    }

    private string InternalGetRemaining()
    {
#if NETCOREAPP
            return _str[_cursor..];
#else
        return _str.Substring(_cursor);
#endif
    }

    public string Remaining => InternalGetRemaining();

    public bool CanRead(int length = 1)
    {
        return _cursor + length <= _str.Length;
    }

    public char Peek(int offset = 0)
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
        return c == SyntaxDoubleQuote || c == SyntaxSingleQuote;
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
            throw new ExpectedTokenException(ReaderTokenType.Integer);
        }

        try
        {
            return int.Parse(number);
        }
        catch (Exception ex)
        {
            if (ex is not (FormatException or OverflowException)) throw;
            _cursor = start;

            throw new InvalidTokenException(ReaderTokenType.Integer, number);
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
            throw new ExpectedTokenException(ReaderTokenType.Long);
        }

        try
        {
            return long.Parse(number);
        }
        catch (Exception ex)
        {
            if (ex is not (FormatException or OverflowException)) throw;
            _cursor = start;

            throw new InvalidTokenException(ReaderTokenType.Long, number);
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
            throw new ExpectedTokenException(ReaderTokenType.Double);
        }

        try
        {
            return double.Parse(number);
        }
        catch (Exception ex)
        {
            if (ex is not (FormatException or OverflowException)) throw;
            _cursor = start;

            throw new InvalidTokenException(ReaderTokenType.Double, number);
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
            throw new ExpectedTokenException(ReaderTokenType.Float);
        }

        try
        {
            return float.Parse(number);
        }
        catch (Exception ex)
        {
            if (ex is not (FormatException or OverflowException)) throw;
            _cursor = start;

            throw new InvalidTokenException(ReaderTokenType.Float, number);
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
            throw new ExpectedTokenException(ReaderTokenType.StartOfQuote);
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
                    Cursor--;
                    throw new InvalidTokenException(ReaderTokenType.Escape, $"{c}");
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

        throw new ExpectedTokenException(ReaderTokenType.EndOfQuote);
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
            throw new ExpectedTokenException(ReaderTokenType.Boolean);
        }

        if (value.Equals("true"))
        {
            return true;
        }
        else if (value.Equals("false"))
        {
            return false;
        }
        else
        {
            _cursor = start;
            throw new InvalidTokenException(ReaderTokenType.Boolean, value);
        }
    }

    public void Expect(char c)
    {
        if (!CanRead() || Peek() != c)
        {
            throw new ExpectedTokenException($"'{c}'");
        }

        Skip();
    }
}