using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Builder;

public interface IArgumentBuilder<T>
{
    public ICommand<T>? Command { get; }
    public CommandNode<T> Build();
}

public interface IArgumentBuilder<TSource, T> : IArgumentBuilder<TSource> where T : IArgumentBuilder<TSource, T>
{
    public T Executes(ICommand<TSource> command);
    
    public T Executes(CommandDelegate<TSource> cmd) => Executes(new CmdImpl(async c =>
    {
        await Task.CompletedTask;
        return cmd(c);
    }));

    public T Executes(CommandDelegateAsync<TSource> cmd) => Executes(new CmdImpl(cmd));

    public T Executes(CommandDelegateNoResult<TSource> cmd) => Executes(new CmdImpl(async c =>
    {
        await cmd(c);
        return 1;
    }));

    public T Then(IArgumentBuilder<TSource> argument);
    public T Then(CommandNode<TSource> argument);
    public T Requires(Predicate<TSource> requirement);
    public T Forward(CommandNode<TSource>? target, RedirectModifier<TSource>? modifier, bool fork);
    
    public T RedirectTo(CommandNode<TSource> target)
    {
        return Forward(target, null, false);
    }

    public T RedirectTo(CommandNode<TSource> target, SingleRedirectModifier<TSource>? modifier)
    {
        return Forward(target, modifier == null ? null : o => new List<TSource> { modifier(o) },
            false);
    }

    public T Fork(CommandNode<TSource> target, RedirectModifier<TSource> modifier)
    {
        return Forward(target, modifier, true);
    }
    
    private class CmdImpl : ICommand<TSource>
    {
        private readonly CommandDelegateAsync<TSource> _del;

        public CmdImpl(CommandDelegateAsync<TSource> del)
        {
            _del = del;
        }

        public Task<int> Run(CommandContext<TSource> context) => _del(context);
    }
}

public abstract class ArgumentBuilder<TSource, T> : IArgumentBuilder<TSource, T> where T : ArgumentBuilder<TSource, T>
{
    private readonly RootCommandNode<TSource> _arguments = new();

    public ICommand<TSource>? Command { get; private set; }
    public Predicate<TSource> Requirement { get; private set; } = _ => true;
    public CommandNode<TSource>? RedirectTarget { get; private set; }
    public RedirectModifier<TSource>? RedirectModifier { get; private set; }
    public bool IsFork { get; private set; }

    public T Then(IArgumentBuilder<TSource> argument)
    {
        if (RedirectTarget != null)
        {
            throw new Exception("Cannot add children to a redirected node");
        }

        _arguments.AddChild(argument.Build());
        return GetThis();
    }

    public T Then(CommandNode<TSource> argument)
    {
        if (RedirectTarget != null)
        {
            throw new Exception("Cannot add children to a redirected node");
        }

        _arguments.AddChild(argument);
        return GetThis();
    }

    public IEnumerable<CommandNode<TSource>> Arguments => _arguments.Children;

    protected abstract T GetThis(); 

    public T Executes(ICommand<TSource> command)
    {
        Command = command;
        return GetThis();
    }

    public T Requires(Predicate<TSource> requirement)
    {
        Requirement = requirement;
        return GetThis();
    }

    public T Forward(CommandNode<TSource>? target, RedirectModifier<TSource>? modifier, bool fork)
    {
        if (_arguments.Children.Any())
        {
            throw new Exception("Cannot forward a node with children");
        }

        RedirectTarget = target;
        RedirectModifier = modifier;
        IsFork = fork;
        return GetThis();
    }

    public abstract CommandNode<TSource> Build();
}