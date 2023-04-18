using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mochi.Brigadier.Builder;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Suggests;

namespace Mochi.Brigadier.Tree;

public abstract class CommandNode<TS> : IComparable<CommandNode<TS>>
{
    private readonly Dictionary<string, CommandNode<TS>> _children = new();
    private Dictionary<string, LiteralCommandNode<TS>> _literals = new();

    private Dictionary<string, ArgumentCommandNode<TS>> _arguments =
        new Dictionary<string, ArgumentCommandNode<TS>>();

    private bool _forks;

    protected CommandNode(ICommand<TS>? command, Predicate<TS> requirement, CommandNode<TS>? redirect,
        RedirectModifier<TS> modifier, bool forks)
    {
        Command = command;
        Requirement = requirement;
        Redirect = redirect;
        RedirectModifier = modifier;
        _forks = forks;
    }

    public ICommand<TS>? Command { get; private set; }

    public IEnumerable<CommandNode<TS>> Children => _children.Values;

    public CommandNode<TS>? GetChild(string name) => _children.GetValueOrDefault(name);

    public CommandNode<TS>? Redirect { get; }

    public RedirectModifier<TS> RedirectModifier { get; }

    public bool CanUse(TS source) => Requirement(source);

    public void AddChild(CommandNode<TS> node)
    {
        if (node is RootCommandNode<TS>)
        {
            throw new NotSupportedException("Cannot add a RootCommandNode as a child to any other CommandNode");
        }

        if (_children.TryGetValue(node.Name, out var child))
        {
            // We've found something to merge onto
            if (node.Command != null)
            {
                child.Command = node.Command;
            }

            foreach (var grandchild in node.Children)
            {
                child.AddChild(grandchild);
            }
        }
        else
        {
            _children.Add(node.Name, node);
            switch (node)
            {
                case LiteralCommandNode<TS> literal:
                    _literals.Add(node.Name, literal);
                    break;
                
                case ArgumentCommandNode<TS> arg:
                    _arguments.Add(node.Name, arg);
                    break;
            }
        }
    }

    public void FindAmbiguities(AmbiguityConsumer<TS> consumer)
    {
        var matches = new HashSet<string>();

        foreach (var child in _children.Values)
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison
            foreach (var sibling in _children.Values
                         .Where(sibling => child != sibling))
            {
                foreach (var input in child.GetExamples()
                             .Where(i => sibling.IsValidInput(i)))
                {
                    matches.Add(input);
                }

                if (!matches.Any()) continue;
                consumer(this, child, sibling, matches);
                matches = new HashSet<string>();
            }

            child.FindAmbiguities(consumer);
        }
    }

    protected abstract bool IsValidInput(string input);

    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (!(o is CommandNode<TS> that)) return false;

        if (!_children.Equals(that._children)) return false;
        if (!Command?.Equals(that.Command) ?? that.Command != null) return false;

        return true;
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return HashCode.Combine(_children, Command);
    }

    public Predicate<TS> Requirement { get; }

    public abstract string Name { get; }

    public abstract string GetUsageText();

    public abstract void Parse(StringReader reader, CommandContextBuilder<TS> contextBuilder);

    public abstract Task<Suggestions> ListSuggestionsAsync(CommandContext<TS> context, SuggestionsBuilder builder);

    public abstract ArgumentBuilder<TS> CreateBuilder();

    protected abstract string GetSortedKey();

    public IEnumerable<CommandNode<TS>> GetRelevantNodes(StringReader input)
    {
        if (_literals.Count <= 0) return _arguments.Values;
        
        var cursor = input.Cursor;
        while (input.CanRead() && input.Peek() != ' ')
        {
            input.Skip();
        }

        var text = input.GetString().Substring(cursor, input.Cursor - cursor);
        input.Cursor = cursor;

        if (_literals.TryGetValue(text, out var literal))
        {
            return new[] { literal };
        }
        
        return _arguments.Values;

    }

    public int CompareTo(CommandNode<TS> o)
    {
        if (this is LiteralCommandNode<TS> == o is LiteralCommandNode<TS>)
        {
            return string.Compare(GetSortedKey(), o.GetSortedKey(), StringComparison.Ordinal);
        }

        return o is LiteralCommandNode<TS> ? 1 : -1;
    }

    public bool IsFork => _forks;

    public abstract IEnumerable<string> GetExamples();
}