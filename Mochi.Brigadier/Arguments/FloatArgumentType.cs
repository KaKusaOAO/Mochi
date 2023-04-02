using System;
using System.Collections.Generic;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Exceptions;

namespace Mochi.Brigadier.Arguments;

public class FloatArgumentType : IArgumentType<float>, IArgumentTypeWithExamples
{
    private static readonly IEnumerable<string> EXAMPLES = new List<string>
        { "0", "1.2", ".5", "-1", "-.5", "-1234.56" };

    private readonly float minimum;
    private readonly float maximum;

    private FloatArgumentType(float minimum, float maximum)
    {
        this.minimum = minimum;
        this.maximum = maximum;
    }

    public static FloatArgumentType FloatArg()
    {
        return FloatArg(float.MinValue);
    }

    public static FloatArgumentType FloatArg(float min)
    {
        return FloatArg(min, float.MaxValue);
    }

    public static FloatArgumentType FloatArg(float min, float max)
    {
        return new FloatArgumentType(min, max);
    }

    public static float GetFloat<T>(CommandContext<T> context, String name)
    {
        return context.GetArgument<float>(name);
    }

    public float GetMinimum()
    {
        return minimum;
    }

    public float GetMaximum()
    {
        return maximum;
    }

    public float Parse(StringReader reader)
    {
        int start = reader.GetCursor();
        float result = reader.ReadFloat();
        if (result < minimum)
        {
            reader.SetCursor(start);
            throw CommandSyntaxException.BuiltInExceptions.FloatTooLow()
                .CreateWithContext(reader, result, minimum);
        }

        if (result > maximum)
        {
            reader.SetCursor(start);
            throw CommandSyntaxException.BuiltInExceptions.FloatTooHigh()
                .CreateWithContext(reader, result, maximum);
        }

        return result;
    }

    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (!(o is FloatArgumentType that)) return false;
        // ReSharper disable CompareOfFloatsByEqualityOperator
        return maximum == that.maximum && minimum == that.minimum;
    }

    public override int GetHashCode() => (int)(31 * minimum + maximum);

    public override string ToString()
    {
        if (minimum == float.MinValue && maximum == float.MaxValue)
        {
            return "float()";
        }

        if (maximum == float.MaxValue)
        {
            return "float(" + minimum + ")";
        }

        return "float(" + minimum + ", " + maximum + ")";
    }

    public IEnumerable<string> GetExamples()
    {
        return EXAMPLES;
    }
}