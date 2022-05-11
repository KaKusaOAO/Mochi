using System.Collections.Generic;
using System.Threading.Tasks;
using KaLib.Brigadier.Context;
using KaLib.Brigadier.Exceptions;
using KaLib.Brigadier.Suggests;

namespace KaLib.Brigadier.Arguments
{
    public class IntegerArgumentType : IArgumentType<int>
    {
        private static readonly IEnumerable<string> Examples = new[] { "0", "123", "-123" };

        private readonly int _minimum;
        private readonly int _maximum;

        private IntegerArgumentType(int minimum, int maximum)
        {
            this._minimum = minimum;
            this._maximum = maximum;
        }

        public static IntegerArgumentType Integer()
        {
            return Integer(int.MinValue);
        }

        public static IntegerArgumentType Integer(int min)
        {
            return Integer(min, int.MaxValue);
        }

        public static IntegerArgumentType Integer(int min, int max)
        {
            return new IntegerArgumentType(min, max);
        }

        public static int GetInteger<TS>(CommandContext<TS> context, string name)
        {
            return context.GetArgument<int>(name);
        }

        public int GetMinimum()
        {
            return _minimum;
        }

        public int GetMaximum()
        {
            return _maximum;
        }

#if NET6_0_OR_GREATER
        public int Parse(StringReader reader)
#else
        public object Parse(StringReader reader)
#endif
        {
            var start = reader.GetCursor();
            var result = reader.ReadInt();
            if (result < _minimum)
            {
                reader.SetCursor(start);
                throw CommandSyntaxException.BuiltInExceptions.IntegerTooLow()
                    .CreateWithContext(reader, result, _minimum);
            }

            if (result > _maximum)
            {
                reader.SetCursor(start);
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
            else if (_maximum == int.MaxValue)
            {
                return "integer(" + _minimum + ")";
            }
            else
            {
                return "integer(" + _minimum + ", " + _maximum + ")";
            }
        }

        public IEnumerable<string> GetExamples()
        {
            return Examples;
        }

#if !NET6_0_OR_GREATER
        public Task<Suggestions> ListSuggestions<TS>(CommandContext<TS> context, SuggestionsBuilder builder) =>
            Suggestions.Empty();
#endif
    }
}