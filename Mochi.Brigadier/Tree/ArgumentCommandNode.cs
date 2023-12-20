using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mochi.Brigadier.Arguments;
using Mochi.Brigadier.Builder;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Exceptions;
using Mochi.Brigadier.Suggests;

namespace Mochi.Brigadier.Tree;

public interface IArgumentCommandNode<T> : ICommandNode<T>
{
    
}

public interface IArgumentCommandNode<TSource, T> : IArgumentCommandNode<TSource>
{
    
}

public class ArgumentCommandNode<TSource, T> : CommandNode<TSource>, IArgumentCommandNode<TSource, T>
{
    protected const string UsageArgumentOpen = "<";
    protected const string UsageArgumentClose = ">";

    public override string Name { get; }
    public IArgumentType<T> Type { get; }
    public SuggestionProvider<TSource>? CustomSuggestions { get; }

    public override ICollection<string> Examples => Type.Examples;

    public ArgumentCommandNode(string name, IArgumentType<T> type, ICommand<TSource>? command, Predicate<TSource> requirement,
        CommandNode<TSource>? redirect, RedirectModifier<TSource>? modifier, bool forks,
        SuggestionProvider<TSource>? customSuggestions) :
        base(command, requirement, redirect, modifier, forks)
    {
        Name = name;
        Type = type;
        CustomSuggestions = customSuggestions;
    }

    public override string GetUsageText()
    {
        return UsageArgumentOpen + Name + UsageArgumentClose;
    }

    public override void Parse(StringReader reader, CommandContextBuilder<TSource> contextBuilder)
    {
        var start = reader.Cursor;
        var result = Type.Parse(reader);
        var parsed = new ParsedArgument<TSource, T>(start, reader.Cursor, result);

        contextBuilder.WithArgument(Name, parsed);
        contextBuilder.WithNode(this, parsed.Range);
    }

    public override Task<Suggestions> ListSuggestionsAsync(CommandContext<TSource> context, SuggestionsBuilder builder)
    {
        if (CustomSuggestions == null)
        {
            return Type.ListSuggestionsAsync(context, builder);
        }

        return CustomSuggestions(context, builder);
    }
    
    public override IArgumentBuilder<TSource> CreateBuilder()
    {
        var builder = RequiredArgumentBuilder<TSource, T>.Argument(Name, Type);
        builder.Requires(Requirement);
        builder.Forward(Redirect, RedirectModifier, IsFork);
        builder.Suggests(CustomSuggestions);
        if (Command != null)
        {
            builder.Executes(Command);
        }

        return builder;
    }

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

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is not ArgumentCommandNode<TSource, T> that) return false;
        if (!Name.Equals(that.Name)) return false;
        return Type.Equals(that.Type) && Equals(o);
    }

    public override int GetHashCode() => HashCode.Combine(Name, Type);

    protected override string GetSortedKey() => Name;

    public override string ToString() => "<argument " + Name + ":" + Type + ">";
}