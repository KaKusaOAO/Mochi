using System;
using System.Collections.Generic;
using System.Linq;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Context;

public class CommandContextBuilder<TS>
{
    private readonly Dictionary<string, ParsedArgument<TS>> _arguments =
        new Dictionary<string, ParsedArgument<TS>>();

    private readonly CommandNode<TS> _rootNode;
    private readonly List<ParsedCommandNode<TS>> _nodes = new List<ParsedCommandNode<TS>>();
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

    public TS GetSource()
    {
        return _source;
    }

    public CommandNode<TS> GetRootNode()
    {
        return _rootNode;
    }

    public CommandContextBuilder<TS> WithArgument(string name, ParsedArgument<TS> argument)
    {
        _arguments.Add(name, argument);
        return this;
    }

    public Dictionary<string, ParsedArgument<TS>> GetArguments()
    {
        return _arguments;
    }

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
        var copy = new CommandContextBuilder<TS>(_dispatcher, _source, _rootNode, _range.GetStart());
        copy._command = _command;
        foreach (var pair in _arguments) copy._arguments.Add(pair.Key, pair.Value);
        copy._nodes.AddRange(_nodes);
        copy._child = _child;
        copy._range = _range;
        copy._forks = _forks;
        return copy;
    }

    public CommandContextBuilder<TS> WithChild(CommandContextBuilder<TS> child)
    {
        _child = child;
        return this;
    }

    public CommandContextBuilder<TS> GetChild()
    {
        return _child;
    }

    public CommandContextBuilder<TS> GetLastChild()
    {
        var result = this;
        while (result.GetChild() != null)
        {
            result = result.GetChild();
        }

        return result;
    }

    public ICommand<TS> GetCommand()
    {
        return _command;
    }

    public List<ParsedCommandNode<TS>> GetNodes()
    {
        return _nodes;
    }

    public CommandContext<TS> Build(string input)
    {
        return new CommandContext<TS>(_source, input, _arguments, _command, _rootNode, _nodes, _range,
            _child == null ? null : _child.Build(input), _modifier, _forks);
    }

    public CommandDispatcher<TS> GetDispatcher()
    {
        return _dispatcher;
    }

    public StringRange GetRange()
    {
        return _range;
    }

    public SuggestionContext<TS> FindSuggestionContext(int cursor)
    {
        if (_range.GetStart() <= cursor)
        {
            if (_range.GetEnd() < cursor)
            {
                if (_child != null)
                {
                    return _child.FindSuggestionContext(cursor);
                }

                if (_nodes.Count > 0)
                {
                    var last = _nodes.Last();
                    return new SuggestionContext<TS>(last.GetNode(), last.GetRange().GetEnd() + 1);
                }

                return new SuggestionContext<TS>(_rootNode, _range.GetStart());
            }

            var prev = _rootNode;
            foreach (var node in _nodes)
            {
                var nodeRange = node.GetRange();
                if (nodeRange.GetStart() <= cursor && cursor <= nodeRange.GetEnd())
                {
                    return new SuggestionContext<TS>(prev, nodeRange.GetStart());
                }

                prev = node.GetNode();
            }

            if (prev == null)
            {
                throw new Exception("Can't find node before cursor");
            }

            return new SuggestionContext<TS>(prev, _range.GetStart());
        }

        throw new Exception("Can't find node before cursor");
    }
}