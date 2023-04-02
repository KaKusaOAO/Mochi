using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier.Arguments;

public class StringArgumentType : IArgumentType<string>, IArgumentTypeWithExamples
{
    private StringType type;

    private StringArgumentType(StringType type)
    {
        this.type = type;
    }

    public static StringArgumentType Word()
    {
        return new StringArgumentType(StringType.SINGLE_WORD);
    }

    public static StringArgumentType String()
    {
        return new StringArgumentType(StringType.QUOTABLE_PHRASE);
    }

    public static StringArgumentType GreedyString()
    {
        return new StringArgumentType(StringType.GREEDY_PHRASE);
    }

    public static string GetString<T>(CommandContext<T> context, string name)
    {
        return context.GetArgument<string>(name);
    }

    public StringType GetStringType()
    {
        return type;
    }

    public string Parse(StringReader reader)
    {
        if (type == StringType.GREEDY_PHRASE)
        {
            var text = reader.GetRemaining();
            reader.SetCursor(reader.GetTotalLength());
            return text;
        }

        if (type == StringType.SINGLE_WORD) return reader.ReadUnquotedString();

        return reader.ReadString();
    }

    public override string ToString()
    {
        return "string()";
    }

    public IEnumerable<string> GetExamples()
    {
        return type.GetExamples();
    }

    public static string EscapeIfRequired(string input) =>
        input.Any(c => !StringReader.IsAllowedInUnquotedString(c)) ? Escape(input) : input;

    private static string Escape(string input)
    {
        var result = new StringBuilder("\"");

        for (int i = 0; i < input.Length; i++)
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
        public static readonly StringType SINGLE_WORD = new StringType("word", "words_with_underscores");
        public static readonly StringType QUOTABLE_PHRASE = new StringType("\"quoted phrase\"", "word", "\"\"");

        public static readonly StringType GREEDY_PHRASE =
            new StringType("word", "words with spaces", "\"and symbols\"");

        private IEnumerable<string> examples;

        private StringType(params string[] examples)
        {
            this.examples = new List<string>(examples);
        }

        public IEnumerable<string> GetExamples()
        {
            return examples;
        }
    }
}