using KaLib.Brigadier.Context;
using KaLib.Brigadier.Tree;

namespace KaLib.Brigadier.Builder;

public abstract class ArgumentBuilder<TS>
{
    public abstract CommandNode<TS> Build();
}

public abstract class ArgumentBuilder<TS, T> : ArgumentBuilder<TS> where T : ArgumentBuilder<TS, T> {
    private readonly RootCommandNode<TS> _arguments = new RootCommandNode<TS>();
    private ICommand<TS> _command;
    private Predicate<TS> _requirement = s => true;
    private CommandNode<TS> _target;
    private RedirectModifier<TS>? _modifier = null;
    private bool _forks;

    protected abstract T GetThis();

    public T Then( ArgumentBuilder<TS> argument) {
        if (_target != null) {
            throw new Exception("Cannot add children to a redirected node");
        }
        _arguments.AddChild(argument.Build());
        return GetThis();
    }

    public T Then( CommandNode<TS> argument) {
        if (_target != null) {
            throw new Exception("Cannot add children to a redirected node");
        }
        _arguments.AddChild(argument);
        return GetThis();
    }

    public IEnumerable<CommandNode<TS>> GetArguments() {
        return _arguments.GetChildren();
    }

    public T Executes(ICommand<TS> command) {
        this._command = command;
        return GetThis();
    }

    public T Executes(CommandDelegate<TS> cmd) => Executes(new CmdImpl(cmd));

    public record CmdImpl(CommandDelegate<TS> Del) : ICommand<TS>
    {
        public int Run(CommandContext<TS> context) => Del(context);
    }

    public ICommand<TS> GetCommand() {
        return _command;
    }

    public T Requires( Predicate<TS> requirement) {
        this._requirement = requirement;
        return GetThis();
    }

    public Predicate<TS> GetRequirement() {
        return _requirement;
    }

    public T Redirect( CommandNode<TS> target) {
        return Forward(target, null, false);
    }

    public T Redirect( CommandNode<TS> target,  SingleRedirectModifier<TS>? modifier) {
        return Forward(target, modifier == null ? null : o => new List<TS> { modifier(o) }, false);
    }

    public T Fork( CommandNode<TS> target,  RedirectModifier<TS>? modifier) {
        return Forward(target, modifier, true);
    }

    public T Forward( CommandNode<TS> target,  RedirectModifier<TS>? modifier,  bool fork) {
        if (_arguments.GetChildren().Any()) {
            throw new Exception("Cannot forward a node with children");
        }
        this._target = target;
        this._modifier = modifier;
        this._forks = fork;
        return GetThis();
    }

    public CommandNode<TS> GetRedirect() {
        return _target;
    }

    public RedirectModifier<TS> GetRedirectModifier() {
        return _modifier;
    }

    public bool IsFork() {
        return _forks;
    }
}