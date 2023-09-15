using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mochi.Brigadier.Builder;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Suggests;

namespace Mochi.Brigadier.Tree;

public interface ICommandNode<T>
{
    public ICommand<T>? Command { get; }
    public IEnumerable<CommandNode<T>> Children { get; }
    public CommandNode<T>? Redirect { get; }
    public RedirectModifier<T>? RedirectModifier { get; }
    public bool IsFork { get; }
    public ICollection<string> Examples { get; }
    public Predicate<T> Requirement { get; }
    public string Name { get; }

    public bool CanUse(T source);
    public void Parse(StringReader reader, CommandContextBuilder<T> contextBuilder);
    public IEnumerable<ICommandNode<T>> GetRelevantNodes(StringReader input);
}

public abstract class CommandNode<T> : ICommandNode<T>, IComparable<CommandNode<T>>
{
    private readonly Dictionary<string, CommandNode<T>> _children = new();
    private Dictionary<string, LiteralCommandNode<T>> _literals = new();
    private Dictionary<string, IArgumentCommandNode<T>> _arguments = new();

    protected CommandNode(ICommand<T>? command, Predicate<T> requirement, CommandNode<T>? redirect,
        RedirectModifier<T>? modifier, bool forks)
    {
        Command = command;
        Requirement = requirement;
        Redirect = redirect;
        RedirectModifier = modifier;
        IsFork = forks;
    }

    public ICommand<T>? Command { get; private set; }

    public IEnumerable<CommandNode<T>> Children => _children.Values;

    public CommandNode<T>? Redirect { get; }

    public RedirectModifier<T>? RedirectModifier { get; }

    public bool IsFork { get; }

    public abstract ICollection<string> Examples { get; }
    
    public Predicate<T> Requirement { get; }

    public abstract string Name { get; }

    public bool CanUse(T source) => Requirement(source);

    public CommandNode<T>? GetChild(string name) => _children.GetValueOrDefault(name);

    public void AddChild(CommandNode<T> node)
    {
        if (node is RootCommandNode<T>)
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
                case LiteralCommandNode<T> literal:
                    _literals.Add(node.Name, literal);
                    break;
                
                case IArgumentCommandNode<T> arg:
                    _arguments.Add(node.Name, arg);
                    break;
            }
        }
    }

    public void FindAmbiguities(AmbiguityConsumer<T> consumer)
    {
        var matches = new HashSet<string>();

        foreach (var child in _children.Values)
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison
            foreach (var sibling in _children.Values
                         .Where(sibling => child != sibling))
            {
                foreach (var input in child.Examples
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

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is not CommandNode<T> that) return false;

        if (!_children.Equals(that._children)) return false;
        if (!Command?.Equals(that.Command) ?? that.Command != null) return false;

        return true;
    }

    public override int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return HashCode.Combine(_children, Command);
    }

    public abstract string GetUsageText();

    public abstract void Parse(StringReader reader, CommandContextBuilder<T> contextBuilder);

    public abstract Task<Suggestions> ListSuggestionsAsync(CommandContext<T> context, SuggestionsBuilder builder);

    public abstract IArgumentBuilder<T> CreateBuilder();

    protected abstract string GetSortedKey();

    public IEnumerable<ICommandNode<T>> GetRelevantNodes(StringReader input)
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

    public int CompareTo(CommandNode<T>? o)
    {
        if (this is LiteralCommandNode<T> == o is LiteralCommandNode<T>)
        {
            return string.Compare(GetSortedKey(), o?.GetSortedKey(), StringComparison.Ordinal);
        }

        return o is LiteralCommandNode<T> ? 1 : -1;
    }
}