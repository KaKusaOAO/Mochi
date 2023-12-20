using System;
using System.Collections.Generic;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Context;

public class CommandContext<T>
{
    private readonly Dictionary<string, IParsedArgument<T>> _arguments;

    public CommandContext<T>? Child { get; }
    public CommandContext<T> LastChild => InternalGetLastChild();
    public ICommand<T>? Command { get; }
    public T Source { get; }
    public RedirectModifier<T>? RedirectModifier { get; }
    public StringRange Range { get; }
    public string Input { get; }
    public CommandNode<T> RootNode { get; }
    public List<ParsedCommandNode<T>> Nodes { get; }

    public CommandContext(T source, string input,
        Dictionary<string, IParsedArgument<T>> arguments,
        ICommand<T>? command, CommandNode<T> rootNode,
        List<ParsedCommandNode<T>> nodes, StringRange range,
        CommandContext<T>? child, RedirectModifier<T>? modifier, bool forks)
    {
        _arguments = arguments;
        
        Source = source;
        Input = input;
        Command = command;
        RootNode = rootNode;
        Nodes = nodes;
        Range = range;
        Child = child;
        RedirectModifier = modifier;
        IsForked = forks;
    }

    public CommandContext<T> CopyFor(T source)
    {
        if (Source?.Equals(source) ?? false)
        {
            return this;
        }

        return new CommandContext<T>(source, Input, _arguments, Command, RootNode, Nodes, Range, Child,
            RedirectModifier, IsForked);
    }


    private CommandContext<T> InternalGetLastChild()
    {
        var result = this;
        while (result.Child != null)
        {
            result = result.Child;
        }

        return result;
    }

    public TValue GetArgument<TValue>(string name)
    {
        var argument = _arguments[name];

        if (argument == null)
        {
            throw new ArgumentException("No such argument '" + name + "' exists on this command");
        }

        var result = argument.Result;
        if (result is TValue v) return v;

        throw new ArgumentException(
            $"Argument '{name}' is defined as {result.GetType().Name}, not {typeof(TValue).Name}");
    }

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is not CommandContext<T> that) return false;

        if (!_arguments.Equals(that._arguments)) return false;
        if (!RootNode.Equals(that.RootNode)) return false;
        if (Nodes.Count != that.Nodes.Count || !Nodes.Equals(that.Nodes)) return false;
        if (!Equals(Command, that.Command)) return false;
        if (!Equals(Source, that.Source)) return false;
        return Equals(Child, that.Child);
    }

    public override int GetHashCode() => 
        HashCode.Combine(Source, _arguments, Command, RootNode, Nodes, Child);

    public bool HasNodes => Nodes.Count > 0;

    public bool IsForked { get; }
}