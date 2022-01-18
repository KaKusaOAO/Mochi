using KaLib.Brigadier.Tree;

namespace KaLib.Brigadier.Context;

public class CommandContext<TS> {
    private readonly TS _source;
    private readonly string _input;
    private readonly ICommand<TS>? _command;
    private readonly Dictionary<string, ParsedArgument<TS>> _arguments;
    private readonly CommandNode<TS> _rootNode;
    private readonly List<ParsedCommandNode<TS>> _nodes;
    private readonly StringRange _range;
    private readonly CommandContext<TS> _child;
    private readonly RedirectModifier<TS>? _modifier;
    private readonly bool _forks;

    public CommandContext(  TS source,  string input, 
        Dictionary<string, ParsedArgument<TS>> arguments, 
        ICommand<TS> command,  CommandNode<TS> rootNode, 
        List<ParsedCommandNode<TS>> nodes,  StringRange range, 
        CommandContext<TS> child,  RedirectModifier<TS> modifier, bool forks) {
        this._source = source;
        this._input = input;
        this._arguments = arguments;
        this._command = command;
        this._rootNode = rootNode;
        this._nodes = nodes;
        this._range = range;
        this._child = child;
        this._modifier = modifier;
        this._forks = forks;
    }

    public CommandContext<TS> CopyFor(TS source) {
        if (this._source?.Equals(source) ?? false) {
            return this;
        }
        return new CommandContext<TS>(source, _input, _arguments, _command, _rootNode, _nodes, _range, _child, _modifier, _forks);
    }

    public CommandContext<TS>? GetChild() {
        return _child;
    }

    public CommandContext<TS> GetLastChild() {
        var result = this;
        while (result.GetChild() != null) {
            result = result.GetChild();
        }
        return result;
    }

    public ICommand<TS>? GetCommand() {
        return _command;
    }

    public TS GetSource() {
        return _source;
    }

    public TV GetArgument<TV>(string name) {
        var argument = _arguments[name];

        if (argument == null) {
            throw new ArgumentException("No such argument '" + name + "' exists on this command");
        }

        var result = argument.GetResult();
        if (result is TV) {
            return (TV) result;
        } else {
            throw new ArgumentException(
                $"Argument '{name}' is defined as {result.GetType().Name}, not {typeof(TV).Name}");
        }
    }

    public override bool Equals(object? o) {
        if (this == o) return true;
        if (o is not CommandContext<TS> that) return false;

        if (!_arguments.Equals(that._arguments)) return false;
        if (!((object)_rootNode).Equals(that._rootNode)) return false;
        if (_nodes.Count != that._nodes.Count || !_nodes.Equals(that._nodes)) return false;
        if (!_command.Equals(that._command)) return false;
        if (!_source!.Equals(that._source)) return false;
        return _child.Equals(that._child);
    }

    public override int GetHashCode() {
        var result = _source!.GetHashCode();
        result = 31 * result + _arguments.GetHashCode();
        result = 31 * result + _command.GetHashCode();
        result = 31 * result + _rootNode.GetHashCode();
        result = 31 * result + _nodes.GetHashCode();
        result = 31 * result + _child.GetHashCode();
        return result;
    }

    public RedirectModifier<TS>? GetRedirectModifier() {
        return _modifier;
    }

    public StringRange GetRange() {
        return _range;
    }

    public string GetInput() {
        return _input;
    }

    public CommandNode<TS> GetRootNode() {
        return _rootNode;
    }

    public List<ParsedCommandNode<TS>> GetNodes() {
        return _nodes;
    }

    public bool HasNodes() {
        return _nodes.Count > 0;
    }

    public bool IsForked() {
        return _forks;
    }
}