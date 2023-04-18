using System;
using System.Collections.Generic;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Exceptions;

namespace Mochi.Brigadier.Arguments;

public class IntegerArgumentType : IArgumentType<int>, IArgumentTypeWithExamples
{
    private static readonly IEnumerable<string> _examples = new[] { "0", "123", "-123" };

    private readonly int _minimum;
    private readonly int _maximum;

    private IntegerArgumentType(int minimum, int maximum)
    {
        _minimum = minimum;
        _maximum = maximum;
    }

    public static IntegerArgumentType Integer(int min = int.MinValue, int max = int.MaxValue) => new(min, max);

    public static int GetInteger<TS>(CommandContext<TS> context, string name) => 
        context.GetArgument<int>(name);

    public int GetMinimum() => _minimum;

    public int GetMaximum() => _maximum;

    public int Parse(StringReader reader)
    {
        var start = reader.Cursor;
        var result = reader.ReadInt();
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
        if (!(o is IntegerArgumentType that)) return false;
        return _maximum == that._maximum && _minimum == that._minimum;
    }

    public override int GetHashCode()
    {
        return 31 * _minimum + _maximum;
    }

    public override string ToString()
    {
        if (_minimum == int.MaxValue && _maximum == int.MaxValue)
        {
            return "integer()";
        }

        if (_maximum == int.MaxValue)
        {
            return "integer(" + _minimum + ")";
        }

        return "integer(" + _minimum + ", " + _maximum + ")";
    }

    public IEnumerable<string> GetExamples()
    {
        return _examples;
    }
}