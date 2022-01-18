using KaLib.Brigadier.Arguments;
using KaLib.Brigadier.Builder;
using KaLib.Brigadier.Context;
using KaLib.Brigadier.Exceptions;
using KaLib.Brigadier.Suggests;

namespace KaLib.Brigadier.Tree;

public class ArgumentCommandNode<TS> : CommandNode<TS>
{
    protected const string UsageArgumentOpen = "<";
    protected const string UsageArgumentClose = ">";

    protected readonly string Name;
    protected readonly IArgumentType Type;
    protected readonly SuggestionProvider<TS> CustomSuggestions;

    public ArgumentCommandNode(string name, IArgumentType type, ICommand<TS> command, Predicate<TS> requirement, CommandNode<TS> redirect, RedirectModifier<TS> modifier, bool forks, SuggestionProvider<TS> customSuggestions) : base(command, requirement, redirect, modifier, forks) {
        this.Name = name;
        this.Type = type;
        this.CustomSuggestions = customSuggestions;
    }

    public IArgumentType GetType() {
        return Type;
    }

    public override string GetName() {
        return Name;
    }

    public override string GetUsageText() {
        return UsageArgumentOpen + Name + UsageArgumentClose;
    }

    public SuggestionProvider<TS> GetCustomSuggestions() {
        return CustomSuggestions;
    }

    public override void Parse(StringReader reader, CommandContextBuilder<TS> contextBuilder) {
        var start = reader.GetCursor();
        var result = Type.Parse(reader);
        var parsed = new ParsedArgument<TS>(start, reader.GetCursor(), result);

        contextBuilder.WithArgument(Name, parsed);
        contextBuilder.WithNode(this, parsed.GetRange());
    }

    public override Task<Suggestions> ListSuggestions(CommandContext<TS> context, SuggestionsBuilder builder) {
        if (CustomSuggestions == null) {
            return Type.ListSuggestions(context, builder);
        } else {
            return CustomSuggestions(context, builder);
        }
    }

    public override ArgumentBuilder<TS> CreateBuilder() => throw new NotImplementedException();

    protected override bool IsValidInput(string input) {
        try {
            var reader = new StringReader(input);
            Type.Parse(reader);
            return !reader.CanRead() || reader.Peek() == ' ';
        } catch (CommandSyntaxException ignored) {
            return false;
        }
    }

    public override bool Equals(object o) {
        if (this == o) return true;
        if (!(o is ArgumentCommandNode<TS> that)) return false;

        if (!Name.Equals(that.Name)) return false;
        if (!Type.Equals(that.Type)) return false;
        return Equals(o);
    }

    public override int GetHashCode() {
        var result = Name.GetHashCode();
        result = 31 * result + Type.GetHashCode();
        return result;
    }

    protected override string GetSortedKey() {
        return Name;
    }

    public override IEnumerable<string> GetExamples() {
        return Type.GetExamples();
    }

    public override string ToString() {
        return "<argument " + Name + ":" + Type +">";
    }
}

public class ArgumentCommandNode<TS, T> : ArgumentCommandNode<TS> 
{
    public ArgumentCommandNode(string name, IArgumentType<T> type, ICommand<TS> command, Predicate<TS> requirement, CommandNode<TS> redirect, RedirectModifier<TS> modifier, bool forks, SuggestionProvider<TS> customSuggestions) : base(name, type, command, requirement, redirect, modifier, forks, customSuggestions)
    {
    }
    
    public override ArgumentBuilder<TS> CreateBuilder() {
        var builder = RequiredArgumentBuilder<TS, T>.Argument(Name, (IArgumentType<T>)Type);
        builder.Requires(GetRequirement());
        builder.Forward(GetRedirect(), GetRedirectModifier(), IsFork());
        builder.Suggests(CustomSuggestions);
        if (GetCommand() != null) {
            builder.Executes(GetCommand());
        }
        return builder;
    }
}
