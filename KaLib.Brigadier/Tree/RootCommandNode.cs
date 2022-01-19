using KaLib.Brigadier.Builder;
using KaLib.Brigadier.Context;
using KaLib.Brigadier.Suggests;

namespace KaLib.Brigadier.Tree;

public class RootCommandNode<TS> : CommandNode<TS> {
    public RootCommandNode() : base(null, c => true, null, s => new[] { s.GetSource() }, false) {
        
    }

    public override string Name => "";

    public override string GetUsageText() {
        return "";
    }

    public override void Parse(StringReader reader, CommandContextBuilder<TS> contextBuilder) {
    }

    public override Task<Suggestions> ListSuggestions(CommandContext<TS> context, SuggestionsBuilder builder) {
        return Suggestions.Empty();
    }

    protected override bool IsValidInput(string input) {
        return false;
    }

    public override bool Equals(object? o) {
        if (this == o) return true;
        if (!(o is RootCommandNode<TS>)) return false;
        return Equals(o);
    }

    public override ArgumentBuilder<TS> CreateBuilder() {
        throw new Exception("Cannot convert root into a builder");
    }

    protected override string GetSortedKey() {
        return "";
    }

    public override IEnumerable<string> GetExamples()
    {
        return Array.Empty<string>();
    }

    public string ToString() {
        return "<root>";
    }
}
