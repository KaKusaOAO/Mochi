using System.Collections.Generic;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Exceptions;

namespace Mochi.Brigadier.Arguments;

public class LongArgumentType : IArgumentType<long>, IArgumentTypeWithExamples
{
    private static readonly IEnumerable<string> _examples = new[] { "0", "123", "-123" };

    private readonly long _minimum;
    private readonly long _maximum;

    private LongArgumentType(long minimum, long maximum)
    {
        _minimum = minimum;
        _maximum = maximum;
    }
    
    public static LongArgumentType LongArg(long min = long.MinValue, long max = long.MaxValue) => new(min, max);

    public static long GetLong<TS>(CommandContext<TS> context, string name) => context.GetArgument<long>(name);

    public long GetMinimum() => _minimum;

    public long GetMaximum() => _maximum;

    public long Parse(StringReader reader)
    {
        var start = reader.Cursor;
        var result = reader.ReadLong();
        if (result < _minimum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.IntegerTooLow()
                .CreateWithContext(reader, result, _minimum);
        }

        if (result > _maximum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.IntegerTooHigh()
                .CreateWithContext(reader, result, _maximum);
        }

        return result;
    }

    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (!(o is LongArgumentType that)) return false;
        return _maximum == that._maximum && _minimum == that._minimum;
    }

    public override int GetHashCode()
    {
        return 31 * _minimum.GetHashCode() + _maximum.GetHashCode();
    }

    public override string ToString()
    {
        if (_minimum == int.MaxValue && _maximum == int.MaxValue)
        {
            return "longArg()";
        }

        if (_maximum == int.MaxValue)
        {
            return "longArg(" + _minimum + ")";
        }

        return "longArg(" + _minimum + ", " + _maximum + ")";
    }

    public IEnumerable<string> GetExamples()
    {
        return _examples;
    }
}