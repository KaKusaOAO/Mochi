using System;
using System.Collections.Generic;
using System.Linq;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Exceptions;

namespace Mochi.Brigadier.Arguments;

public class FloatArgumentType : IArgumentType<float>
{
    private static readonly IEnumerable<string> _examples = new List<string>
        { "0", "1.2", ".5", "-1", "-.5", "-1234.56" };

    private readonly float _minimum;
    private readonly float _maximum;

    private FloatArgumentType(float minimum, float maximum)
    {
        _minimum = minimum;
        _maximum = maximum;
    }

    public static FloatArgumentType FloatArg(float min = float.MinValue, float max = float.MaxValue) => new(min, max);

    public static float GetFloat<T>(CommandContext<T> context, string name) => context.GetArgument<float>(name);

    public float GetMinimum() => _minimum;

    public float GetMaximum() => _maximum;

    public float Parse(StringReader reader)
    {
        var start = reader.Cursor;
        var result = reader.ReadFloat();
        if (result < _minimum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.FloatTooLow()
                .CreateWithContext(reader, result, _minimum);
        }

        if (result > _maximum)
        {
            reader.Cursor = start;
            throw CommandSyntaxException.BuiltInExceptions.FloatTooHigh()
                .CreateWithContext(reader, result, _maximum);
        }

        return result;
    }

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is not FloatArgumentType that) return false;
        // ReSharper disable CompareOfFloatsByEqualityOperator
        return _maximum == that._maximum && _minimum == that._minimum;
    }

    public override int GetHashCode() => (int)(31 * _minimum + _maximum);

    public override string ToString()
    {
        if (_minimum == float.MinValue && _maximum == float.MaxValue)
        {
            return "float()";
        }

        if (_maximum == float.MaxValue)
        {
            return "float(" + _minimum + ")";
        }

        return "float(" + _minimum + ", " + _maximum + ")";
    }

    public ICollection<string> Examples => _examples.ToList();
}