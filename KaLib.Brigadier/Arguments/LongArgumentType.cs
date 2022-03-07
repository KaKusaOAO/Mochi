using System.Collections.Generic;
using System.Threading.Tasks;
using KaLib.Brigadier.Context;
using KaLib.Brigadier.Exceptions;
using KaLib.Brigadier.Suggests;

namespace KaLib.Brigadier.Arguments
{
    public class LongArgumentType : IArgumentType<long>
    {
        private static readonly IEnumerable<string> Examples = new[] { "0", "123", "-123" };

        private readonly long _minimum;
        private readonly long _maximum;

        private LongArgumentType(long minimum, long maximum)
        {
            this._minimum = minimum;
            this._maximum = maximum;
        }

        public static LongArgumentType LongArg()
        {
            return LongArg(long.MinValue);
        }

        public static LongArgumentType LongArg(long min)
        {
            return LongArg(min, long.MaxValue);
        }

        public static LongArgumentType LongArg(long min, long max)
        {
            return new LongArgumentType(min, max);
        }

        public static long GetLong<TS>(CommandContext<TS> context, string name)
        {
            return context.GetArgument<long>(name);
        }

        public long GetMinimum()
        {
            return _minimum;
        }

        public long GetMaximum()
        {
            return _maximum;
        }

#if NET6_0_OR_GREATER
        public long Parse(StringReader reader)
#else
        public object Parse(StringReader reader)
#endif
        {
            var start = reader.GetCursor();
            var result = reader.ReadLong();
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
            else if (_maximum == int.MaxValue)
            {
                return "longArg(" + _minimum + ")";
            }
            else
            {
                return "longArg(" + _minimum + ", " + _maximum + ")";
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