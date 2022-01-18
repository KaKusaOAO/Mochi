using KaLib.Brigadier.Tree;

namespace KaLib.Brigadier.Context;

public class CommandContextBuilder<TS> {
    private readonly Dictionary<string, ParsedArgument<TS>> _arguments = new();
    private readonly CommandNode<TS> _rootNode;
    private readonly List<ParsedCommandNode<TS>> _nodes = new();
    private readonly CommandDispatcher<TS> _dispatcher;
    private TS _source;
    private ICommand<TS> _command;
    private CommandContextBuilder<TS>? _child;
    private StringRange _range;
    private RedirectModifier<TS> _modifier = null;
    private bool _forks;

    public CommandContextBuilder(CommandDispatcher<TS> dispatcher, TS source, CommandNode<TS> rootNode, int start) {
        this._rootNode = rootNode;
        this._dispatcher = dispatcher;
        this._source = source;
        this._range = StringRange.At(start);
    }

    public CommandContextBuilder<TS> WithSource(TS source) {
        this._source = source;
        return this;
    }

    public TS GetSource() {
        return _source;
    }

    public CommandNode<TS> GetRootNode() {
        return _rootNode;
    }

    public CommandContextBuilder<TS> WithArgument(string name, ParsedArgument<TS> argument) {
        this._arguments.Add(name, argument);
        return this;
    }

    public Dictionary<string, ParsedArgument<TS>> GetArguments() {
        return _arguments;
    }

    public CommandContextBuilder<TS> WithCommand(ICommand<TS> command) {
        this._command = command;
        return this;
    }

    public CommandContextBuilder<TS> WithNode(CommandNode<TS> node, StringRange range) {
        _nodes.Add(new ParsedCommandNode<TS>(node, range));
        this._range = StringRange.Encompassing(this._range, range);
        this._modifier = node.GetRedirectModifier();
        this._forks = node.IsFork();
        return this;
    }

    public CommandContextBuilder<TS> Copy() {
        var copy = new CommandContextBuilder<TS>(_dispatcher, _source, _rootNode, _range.GetStart());
        copy._command = _command;
        foreach (var (key, val) in _arguments) copy._arguments.Add(key, val);
        copy._nodes.AddRange(_nodes);
        copy._child = _child;
        copy._range = _range;
        copy._forks = _forks;
        return copy;
    }

    public CommandContextBuilder<TS> WithChild(CommandContextBuilder<TS> child) {
        this._child = child;
        return this;
    }

    public CommandContextBuilder<TS>? GetChild() {
        return _child;
    }

    public CommandContextBuilder<TS> GetLastChild() {
        var result = this;
        while (result.GetChild() != null) {
            result = result.GetChild();
        }
        return result;
    }

    public ICommand<TS> GetCommand() {
        return _command;
    }

    public List<ParsedCommandNode<TS>> GetNodes() {
        return _nodes;
    }

    public CommandContext<TS> Build(string input) {
        return new CommandContext<TS>(_source, input, _arguments, _command, _rootNode, _nodes, _range, _child == null ? null : _child.Build(input), _modifier, _forks);
    }

    public CommandDispatcher<TS> GetDispatcher() {
        return _dispatcher;
    }

    public StringRange GetRange() {
        return _range;
    }

    public SuggestionContext<TS> FindSuggestionContext(int cursor) {
        if (_range.GetStart() <= cursor) {
            if (_range.GetEnd() < cursor) {
                if (_child != null) {
                    return _child.FindSuggestionContext(cursor);
                } else if (_nodes.Count > 0) {
                    var last = _nodes[^1];
                    return new SuggestionContext<TS>(last.GetNode(), last.GetRange().GetEnd() + 1);
                } else {
                    return new SuggestionContext<TS>(_rootNode, _range.GetStart());
                }
            } else {
                var prev = _rootNode;
                foreach (var node in _nodes) {
                    var nodeRange = node.GetRange();
                    if (nodeRange.GetStart() <= cursor && cursor <= nodeRange.GetEnd()) {
                        return new SuggestionContext<TS>(prev, nodeRange.GetStart());
                    }
                    prev = node.GetNode();
                }
                if (prev == null) {
                    throw new Exception("Can't find node before cursor");
                }
                return new SuggestionContext<TS>(prev, _range.GetStart());
            }
        }
        throw new Exception("Can't find node before cursor");
    }
}