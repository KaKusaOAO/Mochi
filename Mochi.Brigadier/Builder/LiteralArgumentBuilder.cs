using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Builder;

public class LiteralArgumentBuilder<T> : ArgumentBuilder<T, LiteralArgumentBuilder<T>>
{
    private readonly string _literal;

    private LiteralArgumentBuilder(string literal)
    {
        _literal = literal;
    }

    public static LiteralArgumentBuilder<T> Literal(string name) => new(name);

    protected override LiteralArgumentBuilder<T> GetThis() => this;

    public string GetLiteral() => _literal;

    public override CommandNode<T> Build()
    {
        var result =
            new LiteralCommandNode<T>(GetLiteral(), Command, Requirement, RedirectTarget, RedirectModifier, IsFork);

        foreach (var argument in Arguments)
        {
            result.AddChild(argument);
        }

        return result;
    }
}