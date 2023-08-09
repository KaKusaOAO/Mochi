using System;

namespace Mochi.Strings;

public class StringParserException : Exception
{
    public StringParserException(string msg) : base(msg) {}
}