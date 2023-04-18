using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mochi.Brigadier.Arguments;
using Mochi.Brigadier.Builder;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Exceptions;
using Mochi.Brigadier.Suggests;

namespace Mochi.Brigadier.Tree;

public class ArgumentCommandNode<TS> : CommandNode<TS>
{
    protected const string UsageArgumentOpen = "<";
    protected const string UsageArgumentClose = ">";

    protected readonly string _name;
    public IArgumentType Type { get; }
    public SuggestionProvider<TS> CustomSuggestions { get; }

    public ArgumentCommandNode(string name, IArgumentType type, ICommand<TS> command, Predicate<TS> requirement,
        CommandNode<TS> redirect, RedirectModifier<TS> modifier, bool forks,
        SuggestionProvider<TS> customSuggestions) :
        base(command, requirement, redirect, modifier, forks)
    {
        _name = name;
        Type = type;
        CustomSuggestions = customSuggestions;
    }

    public override string Name => _name;

    public override string GetUsageText()
    {
        return UsageArgumentOpen + _name + UsageArgumentClose;
    }

    public override void Parse(StringReader reader, CommandContextBuilder<TS> contextBuilder)
    {
        var start = reader.Cursor;
        var result = Type.Parse(reader);
        var parsed = new ParsedArgument<TS>(start, reader.Cursor, result);

        contextBuilder.WithArgument(_name, parsed);
        contextBuilder.WithNode(this, parsed.Range);
    }

    public override Task<Suggestions> ListSuggestionsAsync(CommandContext<TS> context, SuggestionsBuilder builder)
    {
        if (CustomSuggestions == null)
        {
            return Type.ListSuggestions(context, builder);
        }

        return CustomSuggestions(context, builder);
    }

    public override ArgumentBuilder<TS> CreateBuilder() => throw new NotImplementedException();

    protected override bool IsValidInput(string input)
    {
        try
        {
            var reader = new StringReader(input);
            Type.Parse(reader);
            return !reader.CanRead() || reader.Peek() == ' ';
        }
        catch (CommandSyntaxException)
        {
            return false;
        }
    }

    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (!(o is ArgumentCommandNode<TS> that)) return false;

        if (!_name.Equals(that._name)) return false;
        return Type.Equals(that.Type) && Equals(o);
    }

    public override int GetHashCode()
    {
        var result = _name.GetHashCode();
        result = 31 * result + Type.GetHashCode();
        return result;
    }

    protected override string GetSortedKey()
    {
        return _name;
    }

    public override IEnumerable<string> GetExamples()
    {
        return Type.GetExamples();
    }

    public override string ToString()
    {
        return "<argument " + _name + ":" + Type + ">";
    }
}

public class ArgumentCommandNode<TS, T> : ArgumentCommandNode<TS>
{
    public ArgumentCommandNode(string name, IArgumentType<T> type, ICommand<TS> command, Predicate<TS> requirement,
        CommandNode<TS> redirect, RedirectModifier<TS> modifier, bool forks,
        SuggestionProvider<TS> customSuggestions) :
        base(name, type, command, requirement, redirect, modifier, forks, customSuggestions)
    {
    }

    public override ArgumentBuilder<TS> CreateBuilder()
    {
        var builder = RequiredArgumentBuilder<TS, T>.Argument(_name, (IArgumentType<T>)Type);
        builder.Requires(Requirement);
        builder.Forward(Redirect, RedirectModifier, IsFork);
        builder.Suggests(CustomSuggestions);
        if (Command != null)
        {
            builder.Executes(Command);
        }

        return builder;
    }
}