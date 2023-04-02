using System.Collections.Generic;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Exceptions;

namespace Mochi.Brigadier.Arguments;

public class DoubleArgumentType : IArgumentType<double>, IArgumentTypeWithExamples
{
    private static readonly IEnumerable<string> _examples = new List<string>
        { "0", "1.2", ".5", "-1", "-.5", "-1234.56" };

    private readonly double _minimum;
    private readonly double _maximum;

    private DoubleArgumentType(double minimum, double maximum)
    {
        _minimum = minimum;
        _maximum = maximum;
    }

    public static DoubleArgumentType DoubleArg()
    {
        return DoubleArg(double.MinValue);
    }

    public static DoubleArgumentType DoubleArg(double min)
    {
        return DoubleArg(min, double.MaxValue);
    }

    public static DoubleArgumentType DoubleArg(double min, double max)
    {
        return new DoubleArgumentType(min, max);
    }

    public static double GetDouble<T>(CommandContext<T> context, string name)
    {
        return context.GetArgument<double>(name);
    }

    public double GetMinimum()
    {
        return _minimum;
    }

    public double GetMaximum()
    {
        return _maximum;
    }

    public double Parse(StringReader reader)
    {
        var start = reader.Cursor;
        var result = reader.ReadDouble();
        if (result < _minimum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.DoubleTooLow()
                .CreateWithContext(reader, result, _minimum);
        }

        if (result > _maximum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.DoubleTooHigh()
                .CreateWithContext(reader, result, _maximum);
        }

        return result;
    }

    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (!(o is DoubleArgumentType that)) return false;
        // ReSharper disable CompareOfFloatsByEqualityOperator
        return _maximum == that._maximum && _minimum == that._minimum;
    }

    public override int GetHashCode() => (int)(31 * _minimum + _maximum);

    public override string ToString()
    {
        if (_minimum == double.MinValue && _maximum == double.MaxValue)
        {
            return "double()";
        }

        if (_maximum == double.MaxValue)
        {
            return "double(" + _minimum + ")";
        }

        return "double(" + _minimum + ", " + _maximum + ")";
    }

    public IEnumerable<string> GetExamples()
    {
        return _examples;
    }
}