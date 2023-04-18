using System;
using System.Collections.Generic;
using System.Linq;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Context;

public class CommandContextBuilder<TS>
{
    private readonly Dictionary<string, ParsedArgument<TS>> _arguments = new();
    private readonly CommandNode<TS> _rootNode;
    private readonly List<ParsedCommandNode<TS>> _nodes = new();
    private readonly CommandDispatcher<TS> _dispatcher;
    private TS _source;
    private ICommand<TS> _command;
    private CommandContextBuilder<TS> _child;
    private StringRange _range;
    private RedirectModifier<TS> _modifier = null;
    private bool _forks;

    public CommandContextBuilder(CommandDispatcher<TS> dispatcher, TS source, CommandNode<TS> rootNode, int start)
    {
        _rootNode = rootNode;
        _dispatcher = dispatcher;
        _source = source;
        _range = StringRange.At(start);
    }

    public CommandContextBuilder<TS> WithSource(TS source)
    {
        _source = source;
        return this;
    }

    public TS Source => _source;

    public CommandNode<TS> RootNode => _rootNode;

    public CommandContextBuilder<TS> WithArgument(string name, ParsedArgument<TS> argument)
    {
        _arguments.Add(name, argument);
        return this;
    }

    public Dictionary<string, ParsedArgument<TS>> Arguments => _arguments;

    public CommandContextBuilder<TS> WithCommand(ICommand<TS> command)
    {
        _command = command;
        return this;
    }

    public CommandContextBuilder<TS> WithNode(CommandNode<TS> node, StringRange range)
    {
        _nodes.Add(new ParsedCommandNode<TS>(node, range));
        _range = StringRange.Encompassing(_range, range);
        _modifier = node.RedirectModifier;
        _forks = node.IsFork;
        return this;
    }

    public CommandContextBuilder<TS> Copy()
    {
        var copy = new CommandContextBuilder<TS>(_dispatcher, _source, _rootNode, _range.Start)
        {
            _command = _command,
            _child = _child,
            _range = _range,
            _forks = _forks
        };
        
        foreach (var pair in _arguments) copy._arguments.Add(pair.Key, pair.Value);
        copy._nodes.AddRange(_nodes);
        return copy;
    }

    public CommandContextBuilder<TS> WithChild(CommandContextBuilder<TS> child)
    {
        _child = child;
        return this;
    }

    public CommandContextBuilder<TS> Child => _child;

    private CommandContextBuilder<TS> InternalGetLastChild()
    {
        var result = this;
        while (result.Child != null)
        {
            result = result.Child;
        }

        return result;
    }

    public CommandContextBuilder<TS> LastChild => InternalGetLastChild();

    public ICommand<TS> Command => _command;

    public List<ParsedCommandNode<TS>> Nodes => _nodes;

    public CommandContext<TS> Build(string input)
    {
        return new CommandContext<TS>(_source, input, _arguments, _command, _rootNode, _nodes, _range,
            _child == null ? null : _child.Build(input), _modifier, _forks);
    }

    public CommandDispatcher<TS> Dispatcher => _dispatcher;

    public StringRange Range => _range;

    public SuggestionContext<TS> FindSuggestionContext(int cursor)
    {
        if (_range.Start <= cursor)
        {
            if (_range.End < cursor)
            {
                if (_child != null)
                {
                    return _child.FindSuggestionContext(cursor);
                }

                if (_nodes.Count > 0)
                {
                    var last = _nodes.Last();
                    return new SuggestionContext<TS>(last.Node, last.Range.End + 1);
                }

                return new SuggestionContext<TS>(_rootNode, _range.Start);
            }

            var prev = _rootNode;
            foreach (var node in _nodes)
            {
                var nodeRange = node.Range;
                if (nodeRange.Start <= cursor && cursor <= nodeRange.End)
                {
                    return new SuggestionContext<TS>(prev, nodeRange.Start);
                }

                prev = node.Node;
            }

            if (prev == null)
            {
                throw new Exception("Can't find node before cursor");
            }

            return new SuggestionContext<TS>(prev, _range.Start);
        }

        throw new Exception("Can't find node before cursor");
    }
}