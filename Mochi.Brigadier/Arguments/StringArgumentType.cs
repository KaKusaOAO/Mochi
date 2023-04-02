using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier.Arguments;

public class StringArgumentType : IArgumentType<string>, IArgumentTypeWithExamples
{
    private StringType _type;

    private StringArgumentType(StringType type)
    {
        _type = type;
    }

    public static StringArgumentType Word()
    {
        return new StringArgumentType(StringType.SingleWord);
    }

    public static StringArgumentType String()
    {
        return new StringArgumentType(StringType.QuotablePhrase);
    }

    public static StringArgumentType GreedyString()
    {
        return new StringArgumentType(StringType.GreedyPhrase);
    }

    public static string GetString<T>(CommandContext<T> context, string name)
    {
        return context.GetArgument<string>(name);
    }

    public StringType GetStringType()
    {
        return _type;
    }

    public string Parse(StringReader reader)
    {
        if (_type == StringType.GreedyPhrase)
        {
            var text = reader.GetRemaining();
            reader.Cursor = reader.TotalLength;
            return text;
        }

        if (_type == StringType.SingleWord) return reader.ReadUnquotedString();

        return reader.ReadString();
    }

    public override string ToString() => "string()";

    public IEnumerable<string> GetExamples()
    {
        return _type.GetExamples();
    }

    public static string EscapeIfRequired(string input) =>
        input.Any(c => !StringReader.IsAllowedInUnquotedString(c)) ? Escape(input) : input;

    private static string Escape(string input)
    {
        var result = new StringBuilder("\"");

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (c == '\\' || c == '"')
            {
                result.Append('\\');
            }

            result.Append(c);
        }

        result.Append('"');
        return result.ToString();
    }

    public class StringType
    {
        public static readonly StringType SingleWord = new("word", "words_with_underscores");
        public static readonly StringType QuotablePhrase = new("\"quoted phrase\"", "word", "\"\"");
        public static readonly StringType GreedyPhrase = new("word", "words with spaces", "\"and symbols\"");

        private IEnumerable<string> _examples;

        private StringType(params string[] examples)
        {
            _examples = new List<string>(examples);
        }

        public IEnumerable<string> GetExamples() => _examples;
    }
}