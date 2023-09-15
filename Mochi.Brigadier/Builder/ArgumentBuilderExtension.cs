using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Builder;

public static class ArgumentBuilderExtension
{
    public static T RedirectTo<TSource, T>(this IArgumentBuilder<TSource, T> builder, CommandNode<TSource> target) 
        where T : IArgumentBuilder<TSource, T> =>
        builder.RedirectTo(target);
    
    public static T RedirectTo<TSource, T>(this IArgumentBuilder<TSource, T> builder, CommandNode<TSource> target, SingleRedirectModifier<TSource>? modifier) 
        where T : IArgumentBuilder<TSource, T> =>
        builder.RedirectTo(target, modifier);

    public static T Fork<TSource, T>(this IArgumentBuilder<TSource, T> builder, CommandNode<TSource> target, RedirectModifier<TSource> modifier)
        where T : IArgumentBuilder<TSource, T> =>
        builder.Fork(target, modifier);

    public static T Executes<TSource, T>(this IArgumentBuilder<TSource, T> builder, CommandDelegate<TSource> cmd)
        where T : IArgumentBuilder<TSource, T> =>
        builder.Executes(cmd);
    
    public static T Executes<TSource, T>(this IArgumentBuilder<TSource, T> builder, CommandDelegateAsync<TSource> cmd)
        where T : IArgumentBuilder<TSource, T> =>
        builder.Executes(cmd);
    
    public static T Executes<TSource, T>(this IArgumentBuilder<TSource, T> builder, CommandDelegateNoResult<TSource> cmd)
        where T : IArgumentBuilder<TSource, T> =>
        builder.Executes(cmd);
}