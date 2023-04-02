﻿using System.Collections.Generic;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Exceptions;

namespace Mochi.Brigadier.Arguments;

public class DoubleArgumentType : IArgumentType<double>, IArgumentTypeWithExamples
{
    private static readonly IEnumerable<string> EXAMPLES = new List<string>
        { "0", "1.2", ".5", "-1", "-.5", "-1234.56" };

    private readonly double minimum;
    private readonly double maximum;

    private DoubleArgumentType(double minimum, double maximum)
    {
        this.minimum = minimum;
        this.maximum = maximum;
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
        return minimum;
    }

    public double GetMaximum()
    {
        return maximum;
    }

    public double Parse(StringReader reader)
    {
        int start = reader.GetCursor();
        double result = reader.ReadDouble();
        if (result < minimum)
        {
            reader.SetCursor(start);
            throw CommandSyntaxException.BuiltInExceptions.DoubleTooLow()
                .CreateWithContext(reader, result, minimum);
        }

        if (result > maximum)
        {
            reader.SetCursor(start);
            throw CommandSyntaxException.BuiltInExceptions.DoubleTooHigh()
                .CreateWithContext(reader, result, maximum);
        }

        return result;
    }

    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (!(o is DoubleArgumentType that)) return false;
        // ReSharper disable CompareOfFloatsByEqualityOperator
        return maximum == that.maximum && minimum == that.minimum;
    }

    public override int GetHashCode() => (int)(31 * minimum + maximum);

    public override string ToString()
    {
        if (minimum == double.MinValue && maximum == double.MaxValue)
        {
            return "double()";
        }

        if (maximum == double.MaxValue)
        {
            return "double(" + minimum + ")";
        }

        return "double(" + minimum + ", " + maximum + ")";
    }

    public IEnumerable<string> GetExamples()
    {
        return EXAMPLES;
    }
}