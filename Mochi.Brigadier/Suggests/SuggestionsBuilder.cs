using System.Collections.Generic;
using System.Threading.Tasks;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier.Suggests;

public class SuggestionsBuilder
{
    public string Input { get; }
    private readonly string _inputLowerCase;
    public int Start { get; }
    public string Remaining { get; }
    public string RemainingLowerCase { get; }
    private readonly List<Suggestion> _result = new();

    public SuggestionsBuilder(string input, string inputLowerCase, int start)
    {
        Input = input;
        _inputLowerCase = inputLowerCase;
        Start = start;
        Remaining = input[start..];
        RemainingLowerCase = inputLowerCase[start..];
    }

    public SuggestionsBuilder(string input, int start) : this(input, input.ToLowerInvariant(), start)
    {
    }

    public Suggestions Build() => Suggestions.Create(Input, _result);

    public async Task<Suggestions> BuildAsync()
    {
        await Task.Yield();
        return Build();
    }

    public SuggestionsBuilder Suggest(string text, IBrigadierMessage? tooltip = null)
    {
        if (text.Equals(Remaining))
        {
            return this;
        }

        _result.Add(new Suggestion(StringRange.Between(Start, Input.Length), text, tooltip));
        return this;
    }

    public SuggestionsBuilder Suggest(int value, IBrigadierMessage? tooltip = null)
    {
        _result.Add(new IntegerSuggestion(StringRange.Between(Start, Input.Length), value, tooltip));
        return this;
    }

    public SuggestionsBuilder Add(SuggestionsBuilder other)
    {
        _result.AddRange(other._result);
        return this;
    }

    public SuggestionsBuilder CreateOffset(int start) => new(Input, _inputLowerCase, start);
    public SuggestionsBuilder Restart() => CreateOffset(Start);
}