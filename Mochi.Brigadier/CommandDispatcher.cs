using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mochi.Brigadier.Builder;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Exceptions;
using Mochi.Brigadier.Suggests;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier;

/// <summary>
/// The core command dispatcher, for registering, parsing, and executing commands.
/// </summary>
/// <typeparam name="T">a custom "source" type, such as a user or originator of a command</typeparam>
// ReSharper disable PossibleUnintendedReferenceComparison
public class CommandDispatcher<T>
{
    /// <summary>
    /// The string required to separate individual arguments in an input string
    /// </summary>
    /// <seealso cref="ArgumentSeparatorChar"/>
    // ReSharper disable once MemberCanBePrivate.Global
    public const string ArgumentSeparator = " ";

    /// <summary>
    /// The char required to separate individual arguments in an input string
    /// </summary>
    /// <seealso cref="ArgumentSeparator"/>
    // ReSharper disable once MemberCanBePrivate.Global
    public const char ArgumentSeparatorChar = ' ';

    private const string UsageOptionalOpen = "[";
    private const string UsageOptionalClose = "]";
    private const string UsageRequiredOpen = "(";
    private const string UsageRequiredClose = ")";
    private const string UsageOr = "|";

    private readonly RootCommandNode<T> _root;

    // private static readonly Predicate<CommandNode<TS>?> HasCommand = input => input != null && (input.GetCommand() != null || input.Children.Any(x => HasCommand(x)));
    private ResultConsumer<T> _consumer = (_, _, _) => { };

    /// <summary>
    /// Create a new <see cref="CommandDispatcher{TS}"/> with the specified root node.
    /// </summary>
    /// <param name="root">the existing <see cref="RootCommandNode{TS}"/> to use as the basis for this tree</param>
    public CommandDispatcher(RootCommandNode<T> root)
    {
        _root = root;
    }

    /// <summary>
    /// Create a new <see cref="CommandDispatcher{TS}"/> with an empty command tree.
    /// </summary>
    public CommandDispatcher() : this(new RootCommandNode<T>())
    {
    }

    /// <summary>
    /// Utility method for registering new commands.
    /// <para>This is a shortcut for calling <see cref="RootCommandNode{TS}.AddChild"/> after building the provided <paramref name="command"/>.</para>
    /// </summary>
    /// <param name="command">a literal argument builder to add to this command tree</param>
    /// <returns>the node added to this tree</returns>
    public LiteralCommandNode<T> Register(LiteralArgumentBuilder<T> command)
    {
        var build = (LiteralCommandNode<T>)command.Build();
        _root.AddChild(build);
        return build;
    }

    /// <summary>
    /// Sets a callback to be informed of the result of every command.
    /// </summary>
    /// <param name="consumer">the new result consumer to be called</param>
    public void SetConsumer(ResultConsumer<T> consumer)
    {
        _consumer = consumer;
    }

    /// <summary>
    /// Parses and executes a given command.
    /// <para>This is a shortcut to first <see cref="Parse(StringReader,T)"/> and then <see cref="Execute(ParseResults{T})"/></para>
    /// <para>It is recommended to parse and execute as separate steps, as parsing is often the most expensive step, and easiest to cache.</para>
    /// <para>If this command returns a value, then it successfully executed something. If it could not parse the command, or the execution was a failure,
    /// then an exception will be thrown. Most exceptions will be of type <see cref="CommandSyntaxException"/>, but it is possible that a <see cref="Exception"/>
    /// may bubble up from the result of a command. The meaning behind the returned result is arbitrary, and will depend
    /// entirely on what command was performed.</para>
    /// <para>If the command passes through a node that is <see cref="CommandNode{TS}.IsFork"/> then it will be 'forked'.
    /// A forked command will not bubble up any <see cref="CommandSyntaxException"/>s, and the 'result' returned will turn into
    /// 'amount of successful commands executes'.</para>
    /// <para>After each and any command is ran, a registered callback given to <see cref="SetConsumer"/>>
    /// will be notified of the result and success of the command. You can use that method to gather more meaningful
    /// results than this method will return, especially when a command forks.</para>
    /// </summary>
    /// <param name="input">a command string to parse &amp; execute</param>
    /// <param name="source">a custom "source" object, usually representing the originator of this command</param>
    /// <returns>a numeric result from a "command" that was performed</returns>
    /// <exception cref="CommandSyntaxException">if the command failed to parse or execute</exception>
    /// <exception cref="Exception">if the command failed to execute and was not handled gracefully</exception>
    /// <seealso cref="Parse(string,T)"/>
    /// <seealso cref="Parse(StringReader,T)"/>
    /// <seealso cref="Execute(ParseResults{T})"/>
    /// <seealso cref="Execute(StringReader,T)"/>
    public int Execute(string input, T source)
    {
        return Execute(new StringReader(input), source);
    }

    /// <summary>
    /// Parses and executes a given command asynchronously.
    /// <para>This is a shortcut to first <see cref="Parse(StringReader,T)"/> and then <see cref="Execute(ParseResults{T})"/></para>
    /// <para>It is recommended to parse and execute as separate steps, as parsing is often the most expensive step, and easiest to cache.</para>
    /// <para>If this command returns a value, then it successfully executed something. If it could not parse the command, or the execution was a failure,
    /// then an exception will be thrown. Most exceptions will be of type <see cref="CommandSyntaxException"/>, but it is possible that a <see cref="Exception"/>
    /// may bubble up from the result of a command. The meaning behind the returned result is arbitrary, and will depend
    /// entirely on what command was performed.</para>
    /// <para>If the command passes through a node that is <see cref="CommandNode{TS}.IsFork"/> then it will be 'forked'.
    /// A forked command will not bubble up any <see cref="CommandSyntaxException"/>s, and the 'result' returned will turn into
    /// 'amount of successful commands executes'.</para>
    /// <para>After each and any command is ran, a registered callback given to <see cref="SetConsumer"/>>
    /// will be notified of the result and success of the command. You can use that method to gather more meaningful
    /// results than this method will return, especially when a command forks.</para>
    /// </summary>
    /// <param name="input">a command string to parse &amp; execute</param>
    /// <param name="source">a custom "source" object, usually representing the originator of this command</param>
    /// <returns>a numeric result from a "command" that was performed</returns>
    /// <exception cref="CommandSyntaxException">if the command failed to parse or execute</exception>
    /// <exception cref="Exception">if the command failed to execute and was not handled gracefully</exception>
    /// <seealso cref="Parse(string,T)"/>
    /// <seealso cref="Parse(StringReader,T)"/>
    /// <seealso cref="Execute(ParseResults{T})"/>
    /// <seealso cref="Execute(StringReader,T)"/>
    public Task<int> ExecuteAsync(string input, T source)
    {
        return ExecuteAsync(new StringReader(input), source);
    }

    /// <summary>
    /// Parses and executes a given command.
    /// <para>This is a shortcut to first <see cref="Parse(StringReader,T)"/> and then <see cref="Execute(ParseResults{T})"/></para>
    /// <para>It is recommended to parse and execute as separate steps, as parsing is often the most expensive step, and easiest to cache.</para>
    /// <para>If this command returns a value, then it successfully executed something. If it could not parse the command, or the execution was a failure,
    /// then an exception will be thrown. Most exceptions will be of type <see cref="CommandSyntaxException"/>, but it is possible that a <see cref="Exception"/>
    /// may bubble up from the result of a command. The meaning behind the returned result is arbitrary, and will depend
    /// entirely on what command was performed.</para>
    /// <para>If the command passes through a node that is <see cref="CommandNode{TS}.IsFork"/> then it will be 'forked'.
    /// A forked command will not bubble up any <see cref="CommandSyntaxException"/>s, and the 'result' returned will turn into
    /// 'amount of successful commands executes'.</para>
    /// <para>After each and any command is ran, a registered callback given to <see cref="SetConsumer"/>>
    /// will be notified of the result and success of the command. You can use that method to gather more meaningful
    /// results than this method will return, especially when a command forks.</para>
    /// </summary>
    /// <param name="input">a command string to parse &amp; execute</param>
    /// <param name="source">a custom "source" object, usually representing the originator of this command</param>
    /// <returns>a numeric result from a "command" that was performed</returns>
    /// <exception cref="CommandSyntaxException">if the command failed to parse or execute</exception>
    /// <exception cref="Exception">if the command failed to execute and was not handled gracefully</exception>
    /// <seealso cref="Parse(string,T)"/>
    /// <seealso cref="Parse(StringReader,T)"/>
    /// <seealso cref="Execute(ParseResults{T})"/>
    /// <seealso cref="Execute(StringReader,T)"/>
    public int Execute(StringReader input, T source)
    {
        var parse = Parse(input, source);
        return Execute(parse);
    }

    /// <summary>
    /// Parses and executes a given command asynchronously.
    /// <para>This is a shortcut to first <see cref="Parse(StringReader,T)"/> and then <see cref="Execute(ParseResults{T})"/></para>
    /// <para>It is recommended to parse and execute as separate steps, as parsing is often the most expensive step, and easiest to cache.</para>
    /// <para>If this command returns a value, then it successfully executed something. If it could not parse the command, or the execution was a failure,
    /// then an exception will be thrown. Most exceptions will be of type <see cref="CommandSyntaxException"/>, but it is possible that a <see cref="Exception"/>
    /// may bubble up from the result of a command. The meaning behind the returned result is arbitrary, and will depend
    /// entirely on what command was performed.</para>
    /// <para>If the command passes through a node that is <see cref="CommandNode{TS}.IsFork"/> then it will be 'forked'.
    /// A forked command will not bubble up any <see cref="CommandSyntaxException"/>s, and the 'result' returned will turn into
    /// 'amount of successful commands executes'.</para>
    /// <para>After each and any command is ran, a registered callback given to <see cref="SetConsumer"/>>
    /// will be notified of the result and success of the command. You can use that method to gather more meaningful
    /// results than this method will return, especially when a command forks.</para>
    /// </summary>
    /// <param name="input">a command string to parse &amp; execute</param>
    /// <param name="source">a custom "source" object, usually representing the originator of this command</param>
    /// <returns>a numeric result from a "command" that was performed</returns>
    /// <exception cref="CommandSyntaxException">if the command failed to parse or execute</exception>
    /// <exception cref="Exception">if the command failed to execute and was not handled gracefully</exception>
    /// <seealso cref="Parse(string,T)"/>
    /// <seealso cref="Parse(StringReader,T)"/>
    /// <seealso cref="Execute(ParseResults{T})"/>
    /// <seealso cref="Execute(StringReader,T)"/>
    public Task<int> ExecuteAsync(StringReader input, T source)
    {
        var parse = Parse(input, source);
        return ExecuteAsync(parse);
    }

    /// <summary>
    /// Executes a given pre-parsed command.
    /// <para>If this command returns a value, then it successfully executed something. If it could not parse the command, or the execution was a failure,
    /// then an exception will be thrown. Most exceptions will be of type <see cref="CommandSyntaxException"/>, but it is possible that a <see cref="Exception"/>
    /// may bubble up from the result of a command. The meaning behind the returned result is arbitrary, and will depend
    /// entirely on what command was performed.</para>
    /// <para>If the command passes through a node that is <see cref="CommandNode{TS}.IsFork"/> then it will be 'forked'.
    /// A forked command will not bubble up any <see cref="CommandSyntaxException"/>s, and the 'result' returned will turn into
    /// 'amount of successful commands executes'.</para>
    /// <para>After each and any command is ran, a registered callback given to <see cref="SetConsumer"/>>
    /// will be notified of the result and success of the command. You can use that method to gather more meaningful
    /// results than this method will return, especially when a command forks.</para>
    /// </summary>
    /// <param name="parse">the result of a successful <see cref="Parse(StringReader,T)"/>></param>
    /// <returns>a numeric result from a "command" that was performed</returns>
    /// <exception cref="CommandSyntaxException">if the command failed to parse or execute</exception>
    /// <exception cref="Exception">if the command failed to execute and was not handled gracefully</exception>
    /// <seealso cref="Parse(string,T)"/>
    /// <seealso cref="Parse(StringReader,T)"/>
    /// <seealso cref="Execute(ParseResults{T})"/>
    /// <seealso cref="Execute(StringReader,T)"/>
    public int Execute(ParseResults<T> parse) => ExecuteAsync(parse).GetAwaiter().GetResult();

    /// <summary>
    /// Executes a given pre-parsed command asynchronously.
    /// <para>If this command returns a value, then it successfully executed something. If it could not parse the command, or the execution was a failure,
    /// then an exception will be thrown. Most exceptions will be of type <see cref="CommandSyntaxException"/>, but it is possible that a <see cref="Exception"/>
    /// may bubble up from the result of a command. The meaning behind the returned result is arbitrary, and will depend
    /// entirely on what command was performed.</para>
    /// <para>If the command passes through a node that is <see cref="CommandNode{TS}.IsFork"/> then it will be 'forked'.
    /// A forked command will not bubble up any <see cref="CommandSyntaxException"/>s, and the 'result' returned will turn into
    /// 'amount of successful commands executes'.</para>
    /// <para>After each and any command is ran, a registered callback given to <see cref="SetConsumer"/>>
    /// will be notified of the result and success of the command. You can use that method to gather more meaningful
    /// results than this method will return, especially when a command forks.</para>
    /// </summary>
    /// <param name="parse">the result of a successful <see cref="Parse(StringReader,T)"/>></param>
    /// <returns>a numeric result from a "command" that was performed</returns>
    /// <exception cref="CommandSyntaxException">if the command failed to parse or execute</exception>
    /// <exception cref="Exception">if the command failed to execute and was not handled gracefully</exception>
    /// <seealso cref="Parse(string,T)"/>
    /// <seealso cref="Parse(StringReader,T)"/>
    /// <seealso cref="Execute(ParseResults{T})"/>
    /// <seealso cref="Execute(StringReader,T)"/>
    public async Task<int> ExecuteAsync(ParseResults<T> parse)
    {
        if (parse.Reader.CanRead())
        {
            if (parse.Exceptions.Count == 1)
            {
                throw parse.Exceptions.Values.First();
            }

            if (parse.Context.Range.IsEmpty)
            {
                throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownCommand()
                    .CreateWithContext(parse.Reader);
            }

            throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument()
                .CreateWithContext(parse.Reader);
        }

        var result = 0;
        var successfulForks = 0;
        var forked = false;
        var foundCommand = false;
        var command = parse.Reader.GetString();
        var original = parse.Context.Build(command);
        var contexts = new List<CommandContext<T>> { original };
        List<CommandContext<T>>? next = null;

        while (contexts != null)
        {
            var size = contexts.Count;
            for (var i = 0; i < size; i++)
            {
                var context = contexts[i];
                var child = context.Child;
                if (child != null)
                {
                    forked |= context.IsForked;
                    if (child.HasNodes)
                    {
                        foundCommand = true;
                        var modifier = context.RedirectModifier;
                        if (modifier == null)
                        {
                            if (next == null)
                            {
                                next = new List<CommandContext<T>>(1);
                            }

                            next.Add(child.CopyFor(context.Source));
                        }
                        else
                        {
                            try
                            {
                                var results = modifier(context).ToList();
                                if (results.Any())
                                {
                                    next ??= new List<CommandContext<T>>();
                                    next.AddRange(results.Select(source => child.CopyFor(source)));
                                }
                            }
                            catch (CommandSyntaxException)
                            {
                                _consumer(context, false, 0);
                                if (!forked)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                }
                else if (context.Command != null)
                {
                    foundCommand = true;
                    try
                    {
                        var value = await context.Command.Run(context);
                        result += value;
                        _consumer(context, true, value);
                        successfulForks++;
                    }
                    catch (CommandSyntaxException)
                    {
                        _consumer(context, false, 0);
                        if (!forked)
                        {
                            throw;
                        }
                    }
                }
            }

            contexts = next;
            next = null;
        }

        if (!foundCommand)
        {
            _consumer(original, false, 0);
            throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownCommand()
                .CreateWithContext(parse.Reader);
        }

        return forked ? successfulForks : result;
    }

    /// <summary>
    /// Parses a given command.
    /// <para>The result of this method can be cached, and it is advised to do so where appropriate. Parsing is often the
    /// most expensive step, and this allows you to essentially "precompile" a command if it will be ran often.</para>
    /// <para>If the command passes through a node that is <see cref="CommandNode{TS}.IsFork"/> then the resulting context will be marked as 'forked'.
    /// Forked contexts may contain child contexts, which may be modified by the <see cref="RedirectModifier{S}"/> attached to the fork.</para>
    /// <para>Parsing a command can never fail, you will always be provided with a new <see cref="ParseResults{S}"/>.
    /// However, that does not mean that it will always parse into a valid command. You should inspect the returned results
    /// to check for validity. If its <see cref="ParseResults{TS}.Reader"/> <see cref="StringReader.CanRead()"/> then it did not finish
    /// parsing successfully. You can use that position as an indicator to the user where the command stopped being valid.
    /// You may inspect <see cref="ParseResults{TS}.Exceptions"/> if you know the parse failed, as it will explain why it could
    /// not find any valid commands. It may contain multiple exceptions, one for each "potential node" that it could have visited,
    /// explaining why it did not go down that node.</para>
    /// </summary>
    /// <param name="command">a command string to parse</param>
    /// <param name="source">a custom "source" object, usually representing the originator of this command</param>
    /// <returns>the result of parsing this command</returns>
    /// <seealso cref="Parse(StringReader,T)"/>
    /// <seealso cref="Execute(ParseResults{T})"/>
    /// <seealso cref="Execute(string,T)"/>
    public ParseResults<T> Parse(string command, T source) => Parse(new StringReader(command), source);

    /// <summary>
    /// Parses a given command.
    /// <para>The result of this method can be cached, and it is advised to do so where appropriate. Parsing is often the
    /// most expensive step, and this allows you to essentially "precompile" a command if it will be ran often.</para>
    /// <para>If the command passes through a node that is <see cref="CommandNode{TS}.IsFork"/> then the resulting context will be marked as 'forked'.
    /// Forked contexts may contain child contexts, which may be modified by the <see cref="RedirectModifier{S}"/> attached to the fork.</para>
    /// <para>Parsing a command can never fail, you will always be provided with a new <see cref="ParseResults{S}"/>.
    /// However, that does not mean that it will always parse into a valid command. You should inspect the returned results
    /// to check for validity. If its <see cref="ParseResults{TS}.Reader"/> <see cref="StringReader.CanRead()"/> then it did not finish
    /// parsing successfully. You can use that position as an indicator to the user where the command stopped being valid.
    /// You may inspect <see cref="ParseResults{TS}.Exceptions"/> if you know the parse failed, as it will explain why it could
    /// not find any valid commands. It may contain multiple exceptions, one for each "potential node" that it could have visited,
    /// explaining why it did not go down that node.</para>
    /// <para>When you eventually call <see cref="Execute(ParseResults{T})"/> with the result of this method, the above error checking
    /// will occur. You only need to inspect it yourself if you wish to handle that yourself.</para>
    /// </summary>
    /// <param name="command">a command string to parse</param>
    /// <param name="source">a custom "source" object, usually representing the originator of this command</param>
    /// <returns>the result of parsing this command</returns>
    /// <seealso cref="Parse(string,T)"/>
    /// <seealso cref="Execute(ParseResults{T})"/>
    /// <seealso cref="Execute(string,T)"/>
    public ParseResults<T> Parse(StringReader command, T source)
    {
        var context = new CommandContextBuilder<T>(this, source, _root, command.Cursor);
        return ParseNodes(_root, command, context);
    }

    private ParseResults<T> ParseNodes(ICommandNode<T> node, StringReader originalReader,
        CommandContextBuilder<T> contextSoFar)
    {
        var source = contextSoFar.Source;
        var errors = new Dictionary<ICommandNode<T>, CommandSyntaxException>();
        var potentials = new List<ParseResults<T>>();
        var cursor = originalReader.Cursor;

        foreach (var child in node.GetRelevantNodes(originalReader))
        {
            if (!child.CanUse(source)) continue;

            var context = contextSoFar.Copy();
            var reader = new StringReader(originalReader);
            CommandSyntaxException? syntaxException = null;
            
            try
            {
                child.Parse(reader, context);
            }
            catch (Exception ex)
            {
                syntaxException = CommandSyntaxException.BuiltInExceptions.DispatcherParseException()
                    .CreateWithContext(reader, ex.Message, ex);
            }

            if (syntaxException == null && reader.CanRead())
            {
                if (reader.Peek() != ArgumentSeparatorChar)
                {
                    syntaxException = CommandSyntaxException.BuiltInExceptions.DispatcherExpectedArgumentSeparator()
                        .CreateWithContext(reader);
                }
            }
            
            if (syntaxException != null)
            {
                errors[child] = syntaxException;
                reader.Cursor = cursor;
                continue;
            }

            context.WithCommand(child.Command!);
            if (reader.CanRead(child.Redirect == null ? 2 : 1))
            {
                reader.Skip();
                if (child.Redirect != null)
                {
                    var childContext = new CommandContextBuilder<T>(this, source, child.Redirect, reader.Cursor);
                    var parse = ParseNodes(child.Redirect, reader, childContext);
                    context.WithChild(parse.Context);
                    return new ParseResults<T>(context, parse.Reader, parse.Exceptions);
                }
                else
                {
                    var parse = ParseNodes(child, reader, context);
                    potentials.Add(parse);
                }
            }
            else
            {
                potentials.Add(new ParseResults<T>(context, reader,
                    new Dictionary<ICommandNode<T>, CommandSyntaxException>()));
            }
        }

        if (!potentials.Any()) return new ParseResults<T>(contextSoFar, originalReader, errors);
        
        if (potentials.Count > 1)
        {
            potentials.Sort((a, b) =>
            {
                if (!a.Reader.CanRead() && b.Reader.CanRead()) return -1;
                if (a.Reader.CanRead() && !b.Reader.CanRead()) return 1;
                if (a.Exceptions.Count == 0 && b.Exceptions.Count != 0) return -1;
                if (a.Exceptions.Count != 0 && b.Exceptions.Count == 0) return 1;

                return 0;
            });
        }

        return potentials[0];
    }

    /// <summary>
    /// Gets all possible executable commands following the given node.
    ///
    /// <para>You may use <see cref="Root"/> as a target to get all usage data for the entire command tree.</para>
    ///
    /// <para>The returned syntax will be in "simple" form: <c>&lt;param&gt;</c> and <c>literal</c>. "Optional" nodes will be
    /// listed as multiple entries: the parent node, and the child nodes.
    /// For example, a required literal "foo" followed by an optional param "int" will be two nodes:</para>
    /// <ul>
    ///     <li><c>foo</c></li>
    ///     <li><c>foo &lt;int&gt;</c></li>
    /// </ul>
    /// <para>The path to the specified node will <b>not</b> be prepended to the output, as there can theoretically be many
    /// ways to reach a given node. It will only give you paths relative to the specified node, not absolute from root.</para>
    /// </summary>
    /// <param name="node">target node to get child usage strings for</param>
    /// <param name="source">a custom "source" object, usually representing the originator of this command</param>
    /// <param name="restricted">if true, commands that the <paramref name="source"/> cannot access will not be mentioned</param>
    /// <returns>array of full usage strings under the target node</returns>
    public string[] GetAllUsage(CommandNode<T> node, T source, bool restricted)
    {
        var result = new List<string>();
        GetAllUsage(node, source, result, "", restricted);
        return result.ToArray();
    }

#pragma warning disable CS8602
    private void GetAllUsage(CommandNode<T> node, T source, List<string> result, string prefix, bool restricted)
    {
        if (restricted && !node.CanUse(source))
        {
            return;
        }

        if (node.Command != null)
        {
            result.Add(prefix);
        }

        if (node.Redirect != null)
        {
            var redirect = node.Redirect == _root ? "..." : "-> " + node.Redirect.GetUsageText();
            result.Add(string.IsNullOrEmpty(prefix)
                ? node.GetUsageText() + ArgumentSeparator + redirect
                : prefix + ArgumentSeparator + redirect);
        }
        else if (node.Children.Any())
        {
            foreach (var child in node.Children)
            {
                GetAllUsage(child, source, result,
                    string.IsNullOrEmpty(prefix)
                        ? child.GetUsageText()
                        : prefix + ArgumentSeparator + child.GetUsageText(), restricted);
            }
        }
    }
#pragma warning restore CS8602

    /// <summary>
    /// Gets the possible executable commands from a specified node.
    /// <para>You may use <see cref="Root"/> as a target to get all usage data for the entire command tree.</para>
    /// <para>The returned syntax will be in "smart" form: <c>&lt;param&gt;</c>, <c>literal</c>, <c>[optional]</c> and <c>(either|or)</c>.
    /// These forms may be mixed and matched to provide as much information about the child nodes as it can, without being too verbose.
    /// For example, a required literal "foo" followed by an optional param "int" can be compressed into one string:</para>
    /// <ul>
    ///     <li><c>foo [&lt;int&gt;]</c></li>
    /// </ul>
    /// <para>The path to the specified node will <b>not</b> be prepended to the output, as there can theoretically be many
    /// ways to reach a given node. It will only give you paths relative to the specified node, not absolute from root.</para>
    /// </summary>
    /// <param name="node">target node to get child usage strings for</param>
    /// <param name="source">a custom "source" object, usually representing the originator of this command</param>
    /// <returns>array of full usage strings under the target node</returns>
    public Dictionary<CommandNode<T>, string> GetSmartUsage(CommandNode<T> node, T source)
    {
        var result = new Dictionary<CommandNode<T>, string>();

        var optional = node.Command != null;
        foreach (var child in node.Children)
        {
            var usage = GetSmartUsage(child, source, optional, false);
            if (usage != null)
            {
                result.Add(child, usage);
            }
        }

        return result;
    }

    private string? GetSmartUsage(CommandNode<T> node, T source, bool optional, bool deep)
    {
        if (!node.CanUse(source))
        {
            return null;
        }

        var self = optional ? UsageOptionalOpen + node.GetUsageText() + UsageOptionalClose : node.GetUsageText();
        var childOptional = node.Command != null;
        var open = childOptional ? UsageOptionalOpen : UsageRequiredOpen;
        var close = childOptional ? UsageOptionalClose : UsageRequiredClose;

        if (!deep)
        {
            if (node.Redirect != null)
            {
                var redirect = node.Redirect == _root ? "..." : "-> " + node.Redirect.GetUsageText();
                return self + ArgumentSeparator + redirect;
            }

            var children = node.Children.Where(c => c.CanUse(source)).ToList();
            if (children.Count == 1)
            {
                var usage = GetSmartUsage(children.First(), source, childOptional, childOptional);
                if (usage != null)
                {
                    return self + ArgumentSeparator + usage;
                }
            }
            else if (children.Count > 1)
            {
                var childUsage = new HashSet<string>();
                foreach (var child in children)
                {
                    var usage = GetSmartUsage(child, source, childOptional, true);
                    if (usage != null)
                    {
                        childUsage.Add(usage);
                    }
                }

                if (childUsage.Count == 1)
                {
                    var usage = childUsage.First();
                    return self + ArgumentSeparator +
                           (childOptional ? UsageOptionalOpen + usage + UsageOptionalClose : usage);
                }

                if (childUsage.Count > 1)
                {
                    var builder = new StringBuilder(open);
                    var count = 0;
                    foreach (var child in children)
                    {
                        if (count > 0)
                        {
                            builder.Append(UsageOr);
                        }

                        builder.Append(child.GetUsageText());
                        count++;
                    }

                    if (count > 0)
                    {
                        builder.Append(close);
                        return self + ArgumentSeparator + builder;
                    }
                }
            }
        }

        return self;
    }

    /// <summary>
    /// Gets suggestions for a parsed input string on what comes next.
    /// <para>As it is ultimately up to custom argument types to provide suggestions, it may be an asynchronous operation,
    /// for example getting in-game data or player names etc. As such, this method returns a future and no guarantees
    /// are made to when or how the future completes.</para>
    /// <para>The suggestions provided will be in the context of the end of the parsed input string, but may suggest
    /// new or replacement strings for earlier in the input string. For example, if the end of the string was
    /// <c>foobar</c> but an argument preferred it to be <c>minecraft:foobar</c>, it will suggest a replacement for that
    /// whole segment of the input.</para>
    /// </summary>
    /// <param name="parse">the result of a <see cref="Parse(StringReader,T)"/></param>
    /// <returns>a future that will eventually resolve into a <see cref="Suggestions"/> object</returns>
    public Task<Suggestions> GetCompletionSuggestions(ParseResults<T> parse) => 
        GetCompletionSuggestions(parse, parse.Reader.TotalLength);

    public Task<Suggestions> GetCompletionSuggestions(ParseResults<T> parse, int cursor)
    {
        var context = parse.Context;

        var nodeBeforeCursor = context.FindSuggestionContext(cursor);
        var parent = nodeBeforeCursor.Parent;
        var start = Math.Min(nodeBeforeCursor.StartPos, cursor);

        var fullInput = parse.Reader.GetString();
        var truncatedInput = fullInput[..cursor];
        var truncatedInputLowerCase = truncatedInput.ToLowerInvariant();
        var tasks = new Task<Suggestions>[parent.Children.Count()];
        var i = 0;
        foreach (var node in parent.Children)
        {
            var future = Suggestions.Empty();
            try
            {
                future = node.ListSuggestionsAsync(context.Build(truncatedInput),
                    new SuggestionsBuilder(truncatedInput, truncatedInputLowerCase, start));
            }
            catch (CommandSyntaxException)
            {
            }

            tasks[i++] = future;
        }

        var source = new TaskCompletionSource<Suggestions>();
        Task.WhenAll(tasks).ContinueWith(done =>
        {
            var suggestions = done.Result.ToList();
            source.SetResult(Suggestions.Merge(fullInput, suggestions));
        });

        return source.Task;
    }

    /// <summary>
    /// The root node of this command tree.
    /// <para>This is often useful as a target of a <see cref="ArgumentBuilder{TSource,T}.RedirectTarget"/>,
    /// <see cref="GetAllUsage"/> or <see cref="GetSmartUsage"/>.
    /// You may also use it to clone the command tree via <see cref="CommandDispatcher{TS}(RootCommandNode{TS})"/>.</para>
    /// </summary>
    public RootCommandNode<T> Root => _root;

    /// <summary>
    /// Finds a valid path to a given node on the command tree.
    /// <para>There may theoretically be multiple paths to a node on the tree, especially with the use of forking or redirecting.
    /// As such, this method makes no guarantees about which path it finds. It will not look at forks or redirects,
    /// and find the first instance of the target node on the tree.</para>
    /// <para>The only guarantee made is that for the same command tree and the same version of this library, the result of
    /// this method will <b>always</b> be a valid input for <see cref="FindNode"/>, which should return the same node
    /// as provided to this method.</para>
    /// </summary>
    /// <param name="target">the target node you are finding a path for</param>
    /// <returns>a path to the resulting node, or an empty list if it was not found</returns>
    public IEnumerable<string> GetPath(CommandNode<T> target)
    {
        var nodes = new List<List<CommandNode<T>>>();
        AddPaths(_root, nodes, new List<CommandNode<T>>());
        foreach (var list in nodes.Where(list => list[^1] == target))
            return (from node in list where node != _root select node.Name).ToList();
        return Array.Empty<string>();
    }

    /// <summary>
    /// Finds a node by its path.
    /// <para>Paths may be generated with <see cref="GetPath"/>, and are guaranteed (for the same tree, and the
    /// same version of this library) to always produce the same valid node by this method.</para>
    /// <para>If a node could not be found at the specified path, then <c>null</c> will be returned.</para>
    /// </summary>
    /// <param name="path">a generated path to a node</param>
    /// <returns>the node at the given path, or null if not found</returns>
    public CommandNode<T>? FindNode(ICollection<string> path)
    {
        CommandNode<T>? node = _root;
        foreach (var name in path)
        {
            node = node.GetChild(name);
            if (node == null)
            {
                return null;
            }
        }

        return node;
    }

    /// <summary>
    /// Scans the command tree for potential ambiguous commands.
    /// <para>This is a shortcut for <see cref="CommandNode{TS}.FindAmbiguities"/> on <see cref="Root"/>.</para>
    /// <para>Ambiguities are detected by testing every <see cref="CommandNode{TS}.GetExamples"/> on one node verses every sibling
    /// node. This is not fool proof, and relies a lot on the providers of the used argument types to give good examples.</para>
    /// </summary>
    /// <param name="consumer">a callback to be notified of potential ambiguities</param>
    public void FindAmbiguities(AmbiguityConsumer<T> consumer)
    {
        _root.FindAmbiguities(consumer);
    }

    private void AddPaths(CommandNode<T> node, List<List<CommandNode<T>>> result, List<CommandNode<T>> parents)
    {
        var current = new List<CommandNode<T>>(parents) { node };
        result.Add(current);

        foreach (var child in node.Children)
        {
            AddPaths(child, result, current);
        }
    }
}