using KaLib.Brigadier.Context;
using KaLib.Brigadier.Tree;

namespace KaLib.Brigadier.Builder;

public abstract class ArgumentBuilder<TS>
{
    protected readonly RootCommandNode<TS> _arguments = new();
    protected ICommand<TS> _command;
    protected Predicate<TS> _requirement = _ => true;
    protected CommandNode<TS> _target;
    protected RedirectModifier<TS>? _modifier;
    protected bool _forks;

    public ArgumentBuilder<TS> Then(ArgumentBuilder<TS> argument) {
        if (_target != null) {
            throw new Exception("Cannot add children to a redirected node");
        }
        _arguments.AddChild(argument.Build());
        return this;
    }

    public ArgumentBuilder<TS> Then(CommandNode<TS> argument) {
        if (_target != null) {
            throw new Exception("Cannot add children to a redirected node");
        }
        _arguments.AddChild(argument);
        return this;
    }

    public IEnumerable<CommandNode<TS>> GetArguments() {
        return _arguments.GetChildren();
    }

    public ArgumentBuilder<TS> Executes(ICommand<TS> command) {
        this._command = command;
        return this;
    }

    public ArgumentBuilder<TS> Executes(CommandDelegate<TS> cmd) => Executes(new CmdImpl(cmd));

    public record CmdImpl(CommandDelegate<TS> Del) : ICommand<TS>
    {
        public int Run(CommandContext<TS> context) => Del(context);
    }

    public ICommand<TS> Command => _command;

    public ArgumentBuilder<TS> Requires( Predicate<TS> requirement) {
        this._requirement = requirement;
        return this;
    }

    public Predicate<TS> Requirement => _requirement;

    public ArgumentBuilder<TS> Redirect(CommandNode<TS> target) {
        return Forward(target, null, false);
    }

    public ArgumentBuilder<TS> Redirect(CommandNode<TS> target,  SingleRedirectModifier<TS>? modifier) {
        return Forward(target, modifier == null ? null : o => new List<TS> { modifier(o) }, false);
    }

    public ArgumentBuilder<TS> Fork(CommandNode<TS> target,  RedirectModifier<TS>? modifier) {
        return Forward(target, modifier, true);
    }

    public ArgumentBuilder<TS> Forward( CommandNode<TS> target,  RedirectModifier<TS>? modifier,  bool fork) {
        if (_arguments.GetChildren().Any()) {
            throw new Exception("Cannot forward a node with children");
        }
        this._target = target;
        this._modifier = modifier;
        this._forks = fork;
        return this;
    }

    public CommandNode<TS> GetRedirect() {
        return _target;
    }

    public RedirectModifier<TS> RedirectModifier => _modifier;

    public bool IsFork => _forks;

    public abstract CommandNode<TS> Build();
}

public abstract class ArgumentBuilder<TS, T> : ArgumentBuilder<TS> where T : ArgumentBuilder<TS, T>
{
    protected abstract T GetThis();

    public new T Then(ArgumentBuilder<TS> argument) => (T)base.Then(argument);

    public new T Then(CommandNode<TS> argument) => (T)base.Then(argument);
    
    public new T Executes(ICommand<TS> command) => (T)base.Executes(command);

    public new T Executes(CommandDelegate<TS> cmd) => Executes(new CmdImpl(cmd));

    public new T Requires(Predicate<TS> requirement) => (T)base.Requires(requirement);

    public new T Redirect(CommandNode<TS> target) => (T)base.Redirect(target);

    public new T Redirect(CommandNode<TS> target, SingleRedirectModifier<TS>? modifier) =>
        (T)base.Redirect(target, modifier);

    public new T Fork(CommandNode<TS> target, RedirectModifier<TS>? modifier) => (T)base.Fork(target, modifier);

    public new T Forward(CommandNode<TS> target, RedirectModifier<TS>? modifier, bool fork) =>
        (T)base.Forward(target, modifier, fork);
}