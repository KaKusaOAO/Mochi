using System;
using System.Collections.Generic;
using System.Linq;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Context;

public class CommandContextBuilder<T>
{
    private StringRange _range;
    private RedirectModifier<T>? _modifier = null;
    private bool _forks;

    public T Source { get; private set; }
    public Dictionary<string, IParsedArgument<T>> Arguments { get; } = new();
    public CommandNode<T> RootNode { get; }
    public CommandContextBuilder<T>? Child { get; private set; }
    public CommandContextBuilder<T> LastChild => InternalGetLastChild();
    public ICommand<T>? Command { get; private set; }
    public List<ParsedCommandNode<T>> Nodes { get; } = new();
    public CommandDispatcher<T> Dispatcher { get; }
    public StringRange Range => _range;

    public CommandContextBuilder(CommandDispatcher<T> dispatcher, T source, CommandNode<T> rootNode, int start)
    {
        RootNode = rootNode;
        Dispatcher = dispatcher;
        Source = source;
        _range = StringRange.At(start);
    }

    public CommandContextBuilder<T> WithSource(T source)
    {
        Source = source;
        return this;
    }

    public CommandContextBuilder<T> WithArgument<TValue>(string name, ParsedArgument<T, TValue> argument)
    {
        Arguments.Add(name, argument);
        return this;
    }

    public CommandContextBuilder<T> WithCommand(ICommand<T> command)
    {
        Command = command;
        return this;
    }

    public CommandContextBuilder<T> WithNode(CommandNode<T> node, StringRange range)
    {
        Nodes.Add(new ParsedCommandNode<T>(node, range));
        _range = StringRange.Encompassing(_range, range);
        _modifier = node.RedirectModifier;
        _forks = node.IsFork;
        return this;
    }

    public CommandContextBuilder<T> Copy()
    {
        var copy = new CommandContextBuilder<T>(Dispatcher, Source, RootNode, _range.Start)
        {
            Command = Command,
            Child = Child,
            _range = _range,
            _forks = _forks
        };
        
        foreach (var pair in Arguments) copy.Arguments.Add(pair.Key, pair.Value);
        copy.Nodes.AddRange(Nodes);
        return copy;
    }

    public CommandContextBuilder<T> WithChild(CommandContextBuilder<T> child)
    {
        Child = child;
        return this;
    }

    private CommandContextBuilder<T> InternalGetLastChild()
    {
        var result = this;
        while (result.Child != null)
        {
            result = result.Child;
        }

        return result;
    }


    public CommandContext<T> Build(string input)
    {
        return new CommandContext<T>(Source, input, Arguments, Command, RootNode, Nodes, _range,
            Child?.Build(input), _modifier, _forks);
    }

    public SuggestionContext<T> FindSuggestionContext(int cursor)
    {
        if (_range.Start <= cursor)
        {
            if (_range.End < cursor)
            {
                if (Child != null)
                {
                    return Child.FindSuggestionContext(cursor);
                }

                if (Nodes.Count > 0)
                {
                    var last = Nodes.Last();
                    return new SuggestionContext<T>(last.Node, last.Range.End + 1);
                }

                return new SuggestionContext<T>(RootNode, _range.Start);
            }

            var prev = RootNode;
            foreach (var node in Nodes)
            {
                var nodeRange = node.Range;
                if (nodeRange.Start <= cursor && cursor <= nodeRange.End)
                {
                    return new SuggestionContext<T>(prev, nodeRange.Start);
                }

                prev = node.Node;
            }

            if (prev == null)
            {
                throw new Exception("Can't find node before cursor");
            }

            return new SuggestionContext<T>(prev, _range.Start);
        }

        throw new Exception("Can't find node before cursor");
    }
}