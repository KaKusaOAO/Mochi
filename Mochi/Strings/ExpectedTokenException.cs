using System;

namespace Mochi.Strings;

public class ExpectedTokenException : StringParserException
{
    public ExpectedTokenException(ReaderTokenType expected) : base($"Expected {expected.ToFriendlyName()}")
    {
        if (expected == ReaderTokenType.ConstantToken)
            throw new InvalidOperationException("Use new ExpectedTokenException(string) instead.");

        ExpectingToken = expected;
        Expecting = expected.ToFriendlyName();
    }
    
    public ExpectedTokenException(string expected) : base($"Expected {expected}")
    {
        ExpectingToken = ReaderTokenType.ConstantToken;
        Expecting = expected;
    }
    
    public string Expecting { get; }
        
    public ReaderTokenType ExpectingToken { get; }
}