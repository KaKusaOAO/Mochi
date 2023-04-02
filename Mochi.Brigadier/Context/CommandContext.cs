using System;
using System.Collections.Generic;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Context;

public class CommandContext<TS>
{
    private readonly TS _source;
    private readonly string _input;
    private readonly ICommand<TS> _command;
    private readonly Dictionary<string, ParsedArgument<TS>> _arguments;
    private readonly CommandNode<TS> _rootNode;
    private readonly List<ParsedCommandNode<TS>> _nodes;
    private readonly StringRange _range;
    private readonly CommandContext<TS> _child;
    private readonly RedirectModifier<TS> _modifier;
    private readonly bool _forks;

    public CommandContext(TS source, string input,
        Dictionary<string, ParsedArgument<TS>> arguments,
        ICommand<TS> command, CommandNode<TS> rootNode,
        List<ParsedCommandNode<TS>> nodes, StringRange range,
        CommandContext<TS> child, RedirectModifier<TS> modifier, bool forks)
    {
        _source = source;
        _input = input;
        _arguments = arguments;
        _command = command;
        _rootNode = rootNode;
        _nodes = nodes;
        _range = range;
        _child = child;
        _modifier = modifier;
        _forks = forks;
    }

    public CommandContext<TS> CopyFor(TS source)
    {
        if (_source?.Equals(source) ?? false)
        {
            return this;
        }

        return new CommandContext<TS>(source, _input, _arguments, _command, _rootNode, _nodes, _range, _child,
            _modifier, _forks);
    }

    public CommandContext<TS> Child => _child;

    private CommandContext<TS> InternalGetLastChild()
    {
        var result = this;
        while (result.Child != null)
        {
            result = result.Child;
        }

        return result;
    }

    public CommandContext<TS> LastChild => InternalGetLastChild();

    public ICommand<TS> Command => _command;

    public TS Source => _source;

    public TV GetArgument<TV>(string name)
    {
        var argument = _arguments[name];

        if (argument == null)
        {
            throw new ArgumentException("No such argument '" + name + "' exists on this command");
        }

        var result = argument.Result;
        if (result is TV v) return v;

        throw new ArgumentException(
            $"Argument '{name}' is defined as {result.GetType().Name}, not {typeof(TV).Name}");
    }

    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (!(o is CommandContext<TS> that)) return false;

        if (!_arguments.Equals(that._arguments)) return false;
        if (!((object)_rootNode).Equals(that._rootNode)) return false;
        if (_nodes.Count != that._nodes.Count || !_nodes.Equals(that._nodes)) return false;
        if (!_command.Equals(that._command)) return false;
        if (!_source.Equals(that._source)) return false;
        return _child.Equals(that._child);
    }

    public override int GetHashCode()
    {
        var result = _source.GetHashCode();
        result = 31 * result + _arguments.GetHashCode();
        result = 31 * result + _command.GetHashCode();
        result = 31 * result + _rootNode.GetHashCode();
        result = 31 * result + _nodes.GetHashCode();
        result = 31 * result + _child.GetHashCode();
        return result;
    }

    public RedirectModifier<TS> RedirectModifier => _modifier;

    public StringRange Range => _range;

    public string Input => _input;

    public CommandNode<TS> RootNode => _rootNode;

    public List<ParsedCommandNode<TS>> Nodes => _nodes;

    public bool HasNodes()
    {
        return _nodes.Count > 0;
    }

    public bool IsForked => _forks;
}