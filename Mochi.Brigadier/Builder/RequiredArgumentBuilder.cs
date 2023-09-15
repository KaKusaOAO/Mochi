using Mochi.Brigadier.Arguments;
using Mochi.Brigadier.Suggests;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Builder;

public static class RequiredArgumentBuilder<T>
{
    public static RequiredArgumentBuilder<T, TValue> Argument<TValue>(string name, IArgumentType<TValue> type) =>
        RequiredArgumentBuilder<T, TValue>.Argument(name, type);
}

public class RequiredArgumentBuilder<TSource, T> : ArgumentBuilder<TSource, RequiredArgumentBuilder<TSource, T>>
{
    public IArgumentType<T> Type { get; }

    public string Name { get; }
    public SuggestionProvider<TSource>? SuggestionsProvider { get; private set; }
    
    private RequiredArgumentBuilder(string name, IArgumentType<T> type)
    {
        Name = name;
        Type = type;
    }

    public static RequiredArgumentBuilder<TSource, T> Argument(string name, IArgumentType<T> type)
    {
        return new RequiredArgumentBuilder<TSource, T>(name, type);
    }

    public RequiredArgumentBuilder<TSource, T> Suggests(SuggestionProvider<TSource>? provider)
    {
        SuggestionsProvider = provider;
        return this;
    }
    
    protected override RequiredArgumentBuilder<TSource, T> GetThis() => this;

    public override CommandNode<TSource> Build()
    {
        var result = new ArgumentCommandNode<TSource, T>(Name, Type, Command, Requirement, RedirectTarget,
            RedirectModifier, IsFork, SuggestionsProvider);

        foreach (var argument in Arguments)
        {
            result.AddChild(argument);
        }

        return result;
    }
}