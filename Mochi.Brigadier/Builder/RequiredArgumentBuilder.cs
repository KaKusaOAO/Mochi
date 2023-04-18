using Mochi.Brigadier.Arguments;
using Mochi.Brigadier.Suggests;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Builder;

public class RequiredArgumentBuilder<TS, T> : ArgumentBuilder<TS, RequiredArgumentBuilder<TS, T>>
{
    private readonly string _name;
    private readonly IArgumentType<T> _type;
    private SuggestionProvider<TS> _suggestionsProvider = null;

    private RequiredArgumentBuilder(string name, IArgumentType<T> type)
    {
        _name = name;
        _type = type;
    }

    public static RequiredArgumentBuilder<TS, T> Argument(string name, IArgumentType<T> type)
    {
        return new RequiredArgumentBuilder<TS, T>(name, type);
    }

    public RequiredArgumentBuilder<TS, T> Suggests(SuggestionProvider<TS> provider)
    {
        _suggestionsProvider = provider;
        return this;
    }

    public SuggestionProvider<TS> GetSuggestionsProvider() => _suggestionsProvider;

    protected override RequiredArgumentBuilder<TS, T> GetThis() => this;

    public IArgumentType<T> Type => _type;

    public string GetName() => _name;

    public override CommandNode<TS> Build()
    {
        var result = new ArgumentCommandNode<TS, T>(GetName(), Type, Command, Requirement, GetRedirect(),
            RedirectModifier, IsFork, GetSuggestionsProvider());

        foreach (var argument in Arguments)
        {
            result.AddChild(argument);
        }

        return result;
    }
}