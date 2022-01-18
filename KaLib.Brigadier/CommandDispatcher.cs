using System.Collections.ObjectModel;
using System.Text;
using KaLib.Brigadier.Builder;
using KaLib.Brigadier.Context;
using KaLib.Brigadier.Exceptions;
using KaLib.Brigadier.Suggests;
using KaLib.Brigadier.Tree;

namespace KaLib.Brigadier;

/// <summary>
/// The core command dispatcher, for registering, parsing, and executing commands.
/// </summary>
/// <typeparam name="S">a custom "source" type, such as a user or originator of a command</typeparam>
public class CommandDispatcher<TS> {
    /// <summary>
    /// The string required to separate individual arguments in an input string
    /// </summary>
    /// <seealso cref="ArgumentSeparatorChar"/>
    public static readonly string ArgumentSeparator = " ";
    
    /// <summary>
    /// The char required to separate individual arguments in an input string
    /// </summary>
    /// <seealso cref="ArgumentSeparator"/>
    public static readonly char ArgumentSeparatorChar = ' ';

    private static readonly string UsageOptionalOpen = "[";
    private static readonly string UsageOptionalClose = "]";
    private static readonly string UsageRequiredOpen = "(";
    private static readonly string UsageRequiredClose = ")";
    private static readonly string UsageOr = "|";

    private readonly RootCommandNode<TS> _root;

    private static readonly Predicate<CommandNode<TS>> HasCommand = input => input != null && (input.GetCommand() != null || input.GetChildren().Any(x => HasCommand(x)));
    private ResultConsumer<TS> _consumer = (c, s, r) => { };

    /**
     * Create a new {@link CommandDispatcher} with the specified root node.
     *
     * <p>This is often useful to copy existing or pre-defined command trees.</p>
     *
     * @param root the existing {@link RootCommandNode} to use as the basis for this tree
     */
    public CommandDispatcher(RootCommandNode<TS> root) {
        this._root = root;
    }

    /**
     * Creates a new {@link CommandDispatcher} with an empty command tree.
     */
    public CommandDispatcher() : this(new RootCommandNode<TS>()) {
        
    }

    /**
     * Utility method for registering new commands.
     *
     * <p>This is a shortcut for calling {@link RootCommandNode#addChild(CommandNode)} after building the provided {@code command}.</p>
     *
     * <p>As {@link RootCommandNode} can only hold literals, this method will only allow literal arguments.</p>
     *
     * @param command a literal argument builder to add to this command tree
     * @return the node added to this tree
     */
    public LiteralCommandNode<TS> Register(LiteralArgumentBuilder<TS> command) {
        var build = (LiteralCommandNode<TS>) command.Build();
        _root.AddChild(build);
        return build;
    }

    /**
     * Sets a callback to be informed of the result of every command.
     *
     * @param consumer the new result consumer to be called
     */
    public void SetConsumer(ResultConsumer<TS> consumer) {
        this._consumer = consumer;
    }

    /**
     * Parses and executes a given command.
     *
     * <p>This is a shortcut to first {@link #parse(StringReader, Object)} and then {@link #execute(ParseResults)}.</p>
     *
     * <p>It is recommended to parse and execute as separate steps, as parsing is often the most expensive step, and easiest to cache.</p>
     *
     * <p>If this command returns a value, then it successfully executed something. If it could not parse the command, or the execution was a failure,
     * then an exception will be thrown. Most exceptions will be of type {@link CommandSyntaxException}, but it is possible that a {@link RuntimeException}
     * may bubble up from the result of a command. The meaning behind the returned result is arbitrary, and will depend
     * entirely on what command was performed.</p>
     *
     * <p>If the command passes through a node that is {@link CommandNode#isFork()} then it will be 'forked'.
     * A forked command will not bubble up any {@link CommandSyntaxException}s, and the 'result' returned will turn into
     * 'amount of successful commands executes'.</p>
     *
     * <p>After each and any command is ran, a registered callback given to {@link #setConsumer(ResultConsumer)}
     * will be notified of the result and success of the command. You can use that method to gather more meaningful
     * results than this method will return, especially when a command forks.</p>
     *
     * @param input a command string to parse &amp; execute
     * @param source a custom "source" object, usually representing the originator of this command
     * @return a numeric result from a "command" that was performed
     * @throws CommandSyntaxException if the command failed to parse or execute
     * @throws RuntimeException if the command failed to execute and was not handled gracefully
     * @see #parse(String, Object)
     * @see #parse(StringReader, Object)
     * @see #execute(ParseResults)
     * @see #execute(StringReader, Object)
     */
    public int Execute(string input, TS source) {
        return Execute(new StringReader(input), source);
    }

    /**
     * Parses and executes a given command.
     *
     * <p>This is a shortcut to first {@link #parse(StringReader, Object)} and then {@link #execute(ParseResults)}.</p>
     *
     * <p>It is recommended to parse and execute as separate steps, as parsing is often the most expensive step, and easiest to cache.</p>
     *
     * <p>If this command returns a value, then it successfully executed something. If it could not parse the command, or the execution was a failure,
     * then an exception will be thrown. Most exceptions will be of type {@link CommandSyntaxException}, but it is possible that a {@link RuntimeException}
     * may bubble up from the result of a command. The meaning behind the returned result is arbitrary, and will depend
     * entirely on what command was performed.</p>
     *
     * <p>If the command passes through a node that is {@link CommandNode#isFork()} then it will be 'forked'.
     * A forked command will not bubble up any {@link CommandSyntaxException}s, and the 'result' returned will turn into
     * 'amount of successful commands executes'.</p>
     *
     * <p>After each and any command is ran, a registered callback given to {@link #setConsumer(ResultConsumer)}
     * will be notified of the result and success of the command. You can use that method to gather more meaningful
     * results than this method will return, especially when a command forks.</p>
     *
     * @param input a command string to parse &amp; execute
     * @param source a custom "source" object, usually representing the originator of this command
     * @return a numeric result from a "command" that was performed
     * @throws CommandSyntaxException if the command failed to parse or execute
     * @throws RuntimeException if the command failed to execute and was not handled gracefully
     * @see #parse(String, Object)
     * @see #parse(StringReader, Object)
     * @see #execute(ParseResults)
     * @see #execute(String, Object)
     */
    public int Execute(StringReader input, TS source) {
        var parse = this.Parse(input, source);
        return Execute(parse);
    }

    /**
     * Executes a given pre-parsed command.
     *
     * <p>If this command returns a value, then it successfully executed something. If the execution was a failure,
     * then an exception will be thrown.
     * Most exceptions will be of type {@link CommandSyntaxException}, but it is possible that a {@link RuntimeException}
     * may bubble up from the result of a command. The meaning behind the returned result is arbitrary, and will depend
     * entirely on what command was performed.</p>
     *
     * <p>If the command passes through a node that is {@link CommandNode#isFork()} then it will be 'forked'.
     * A forked command will not bubble up any {@link CommandSyntaxException}s, and the 'result' returned will turn into
     * 'amount of successful commands executes'.</p>
     *
     * <p>After each and any command is ran, a registered callback given to {@link #setConsumer(ResultConsumer)}
     * will be notified of the result and success of the command. You can use that method to gather more meaningful
     * results than this method will return, especially when a command forks.</p>
     *
     * @param parse the result of a successful {@link #parse(StringReader, Object)}
     * @return a numeric result from a "command" that was performed.
     * @throws CommandSyntaxException if the command failed to parse or execute
     * @throws RuntimeException if the command failed to execute and was not handled gracefully
     * @see #parse(String, Object)
     * @see #parse(StringReader, Object)
     * @see #execute(String, Object)
     * @see #execute(StringReader, Object)
     */
    public int Execute(ParseResults<TS> parse) {
        if (parse.GetReader().CanRead()) {
            if (parse.GetExceptions().Count == 1) {
                throw parse.GetExceptions().Values.First();
            } else if (parse.GetContext().GetRange().IsEmpty()) {
                throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownCommand().CreateWithContext(parse.GetReader());
            } else {
                throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownArgument().CreateWithContext(parse.GetReader());
            }
        }

        var result = 0;
        var successfulForks = 0;
        var forked = false;
        var foundCommand = false;
        var command = parse.GetReader().GetString();
        var original = parse.GetContext().Build(command);
        var contexts = new List<CommandContext<TS>> { original };
        List<CommandContext<TS>> next = null;

        while (contexts != null) {
            var size = contexts.Count;
            for (var i = 0; i < size; i++) {
                var context = contexts[i];
                var child = context.GetChild();
                if (child != null) {
                    forked |= context.IsForked();
                    if (child.HasNodes()) {
                        foundCommand = true;
                        var modifier = context.GetRedirectModifier();
                        if (modifier == null) {
                            if (next == null) {
                                next = new List<CommandContext<TS>>(1);
                            }
                            next.Add(child.CopyFor(context.GetSource()));
                        } else {
                            try {
                                var results = modifier(context);
                                if (results.Any()) {
                                    if (next == null) {
                                        next = new List<CommandContext<TS>>();
                                    }
                                    foreach (var source in results) {
                                        next.Add(child.CopyFor(source));
                                    }
                                }
                            } catch (CommandSyntaxException ex) {
                                _consumer(context, false, 0);
                                if (!forked) {
                                    throw ex;
                                }
                            }
                        }
                    }
                } else if (context.GetCommand() != null) {
                    foundCommand = true;
                    try {
                        var value = context.GetCommand()!.Run(context);
                        result += value;
                        _consumer(context, true, value);
                        successfulForks++;
                    } catch (CommandSyntaxException ex) {
                        _consumer(context, false, 0);
                        if (!forked) {
                            throw ex;
                        }
                    }
                }
            }

            contexts = next;
            next = null;
        }

        if (!foundCommand) {
            _consumer(original, false, 0);
            throw CommandSyntaxException.BuiltInExceptions.DispatcherUnknownCommand().CreateWithContext(parse.GetReader());
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
    /// to check for validity. If its <see cref="ParseResults{TS}.GetReader"/> <see cref="StringReader.CanRead()"/> then it did not finish
    /// parsing successfully. You can use that position as an indicator to the user where the command stopped being valid.
    /// You may inspect <see cref="ParseResults{TS}.GetExceptions"/> if you know the parse failed, as it will explain why it could
    /// not find any valid commands. It may contain multiple exceptions, one for each "potential node" that it could have visited,
    /// explaining why it did not go down that node.</para>
    /// </summary>
    /// <param name="command">a command string to parse</param>
    /// <param name="source">a custom "source" object, usually representing the originator of this command</param>
    /// <returns>the result of parsing this command</returns>
    /// <seealso cref="Parse(KaLib.Brigadier.StringReader,TS)"/>
    /// <seealso cref="Execute(ParseResults{S})"/>
    /// <seealso cref="Execute(string,S)"/>
    public ParseResults<TS> Parse(string command, TS source) {
        return Parse(new StringReader(command), source);
    }

    /// <summary>
    /// Parses a given command.
    /// <para>The result of this method can be cached, and it is advised to do so where appropriate. Parsing is often the
    /// most expensive step, and this allows you to essentially "precompile" a command if it will be ran often.</para>
    /// <para>If the command passes through a node that is <see cref="CommandNode{TS}.IsFork"/> then the resulting context will be marked as 'forked'.
    /// Forked contexts may contain child contexts, which may be modified by the <see cref="RedirectModifier{S}"/> attached to the fork.</para>
    /// <para>Parsing a command can never fail, you will always be provided with a new <see cref="ParseResults{S}"/>.
    /// However, that does not mean that it will always parse into a valid command. You should inspect the returned results
    /// to check for validity. If its <see cref="ParseResults{TS}.GetReader"/> <see cref="StringReader.CanRead()"/> then it did not finish
    /// parsing successfully. You can use that position as an indicator to the user where the command stopped being valid.
    /// You may inspect <see cref="ParseResults{TS}.GetExceptions"/> if you know the parse failed, as it will explain why it could
    /// not find any valid commands. It may contain multiple exceptions, one for each "potential node" that it could have visited,
    /// explaining why it did not go down that node.</para>
    /// <para>When you eventually call <see cref="Execute(ParseResults{S})"/> with the result of this method, the above error checking
    /// will occur. You only need to inspect it yourself if you wish to handle that yourself.</para>
    /// </summary>
    /// <param name="command">a command string to parse</param>
    /// <param name="source">a custom "source" object, usually representing the originator of this command</param>
    /// <returns>the result of parsing this command</returns>
    /// <seealso cref="Parse(string,TS)"/>
    /// <seealso cref="Execute(ParseResults{S})"/>
    /// <seealso cref="Execute(string,S)"/>
    public ParseResults<TS> Parse(StringReader command, TS source) {
        var context = new CommandContextBuilder<TS>(this, source, _root, command.GetCursor());
        return ParseNodes(_root, command, context);
    }

    private ParseResults<TS> ParseNodes(CommandNode<TS> node, StringReader originalReader, CommandContextBuilder<TS> contextSoFar) {
        var source = contextSoFar.GetSource();
        Dictionary<CommandNode<TS>, CommandSyntaxException> errors = null;
        List<ParseResults<TS>> potentials = null;
        var cursor = originalReader.GetCursor();

        foreach (var child in node.GetRelevantNodes(originalReader)) {
            if (!child.CanUse(source)) {
                continue;
            }
            var context = contextSoFar.Copy();
            var reader = new StringReader(originalReader);
            try {
                try {
                    child.Parse(reader, context);
                } catch (Exception ex) {
                    throw CommandSyntaxException.BuiltInExceptions.DispatcherParseException().CreateWithContext(reader, ex.Message);
                }
                if (reader.CanRead()) {
                    if (reader.Peek() != ArgumentSeparatorChar) {
                        throw CommandSyntaxException.BuiltInExceptions.DispatcherExpectedArgumentSeparator().CreateWithContext(reader);
                    }
                }
            } catch (CommandSyntaxException ex) {
                if (errors == null) {
                    errors = new Dictionary<CommandNode<TS>, CommandSyntaxException>();
                }
                errors.Add(child, ex);
                reader.SetCursor(cursor);
                continue;
            }

            context.WithCommand(child.GetCommand()!);
            if (reader.CanRead(child.GetRedirect() == null ? 2 : 1)) {
                reader.Skip();
                if (child.GetRedirect() != null) {
                    var childContext = new CommandContextBuilder<TS>(this, source, child.GetRedirect(), reader.GetCursor());
                    var parse = ParseNodes(child.GetRedirect(), reader, childContext);
                    context.WithChild(parse.GetContext());
                    return new ParseResults<TS>(context, parse.GetReader(), parse.GetExceptions());
                } else {
                    var parse = ParseNodes(child, reader, context);
                    if (potentials == null) {
                        potentials = new List<ParseResults<TS>>();
                    }
                    potentials.Add(parse);
                }
            } else {
                if (potentials == null) {
                    potentials = new List<ParseResults<TS>>();
                }
                potentials.Add(new ParseResults<TS>(context, reader, new Dictionary<CommandNode<TS>,CommandSyntaxException>()));
            }
        }

        if (potentials != null) {
            if (potentials.Count > 1) {
                potentials.Sort((a, b) => {
                    if (!a.GetReader().CanRead() && b.GetReader().CanRead()) {
                        return -1;
                    }
                    if (a.GetReader().CanRead() && !b.GetReader().CanRead()) {
                        return 1;
                    }
                    if (a.GetExceptions().Count == 0 && b.GetExceptions().Count != 0) {
                        return -1;
                    }
                    if (a.GetExceptions().Count != 0 && b.GetExceptions().Count == 0) {
                        return 1;
                    }
                    return 0;
                });
            }
            return potentials[0];
        }

        return new ParseResults<TS>(contextSoFar, originalReader, errors ?? new Dictionary<CommandNode<TS>, CommandSyntaxException>());
    }

    /// <summary>
    /// Gets all possible executable commands following the given node.
    ///
    /// <para>You may use <see cref="GetRoot"/> as a target to get all usage data for the entire command tree.</para>
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
    /// <param name="restricted">if true, commands that the <c>source</c> cannot access will not be mentioned</param>
    /// <returns>array of full usage strings under the target node</returns>
    public string[] GetAllUsage(CommandNode<TS> node, TS source, bool restricted) {
        List<string> result = new ();
        GetAllUsage(node, source, result, "", restricted);
        return result.ToArray();
    }

    private void GetAllUsage(CommandNode<TS> node, TS source, List<string> result, string prefix, bool restricted) {
        if (restricted && !node.CanUse(source)) {
            return;
        }

        if (node.GetCommand() != null) {
            result.Add(prefix);
        }

        if (node.GetRedirect() != null) {
            var redirect = node.GetRedirect() == _root ? "..." : "-> " + node.GetRedirect().GetUsageText();
            result.Add(string.IsNullOrEmpty(prefix) ? node.GetUsageText() + ArgumentSeparator + redirect : prefix + ArgumentSeparator + redirect);
        } else if (node.GetChildren().Any()) {
            foreach (var child in node.GetChildren()) {
                GetAllUsage(child, source, result, string.IsNullOrEmpty(prefix) ? child.GetUsageText() : prefix + ArgumentSeparator + child.GetUsageText(), restricted);
            }
        }
    }

    /**
     * Gets the possible executable commands from a specified node.
     *
     * <p>You may use {@link #getRoot()} as a target to get usage data for the entire command tree.</p>
     *
     * <p>The returned syntax will be in "smart" form: {@code <param>}, {@code literal}, {@code [optional]} and {@code (either|or)}.
     * These forms may be mixed and matched to provide as much information about the child nodes as it can, without being too verbose.
     * For example, a required literal "foo" followed by an optional param "int" can be compressed into one string:</p>
     * <ul>
     *     <li>{@code foo [<int>]}</li>
     * </ul>
     *
     * <p>The path to the specified node will <b>not</b> be prepended to the output, as there can theoretically be many
     * ways to reach a given node. It will only give you paths relative to the specified node, not absolute from root.</p>
     *
     * <p>The returned usage will be restricted to only commands that the provided {@code source} can use.</p>
     *
     * @param node target node to get child usage strings for
     * @param source a custom "source" object, usually representing the originator of this command
     * @return array of full usage strings under the target node
     */
    public Dictionary<CommandNode<TS>, string> GetSmartUsage(CommandNode<TS> node, TS source) {
        Dictionary<CommandNode<TS>, string> result = new();

        var optional = node.GetCommand() != null;
        foreach (var child in node.GetChildren()) {
            var usage = GetSmartUsage(child, source, optional, false);
            if (usage != null) {
                result.Add(child, usage);
            }
        }
        return result;
    }

    private string? GetSmartUsage(CommandNode<TS> node, TS source, bool optional, bool deep) {
        if (!node.CanUse(source)) {
            return null;
        }

        var self = optional ? UsageOptionalOpen + node.GetUsageText() + UsageOptionalClose : node.GetUsageText();
        var childOptional = node.GetCommand() != null;
        var open = childOptional ? UsageOptionalOpen : UsageRequiredOpen;
        var close = childOptional ? UsageOptionalClose : UsageRequiredClose;

        if (!deep) {
            if (node.GetRedirect() != null) {
                var redirect = node.GetRedirect() == _root ? "..." : "-> " + node.GetRedirect().GetUsageText();
                return self + ArgumentSeparator + redirect;
            } else {
                var children = node.GetChildren().Where(c => c.CanUse(source)).ToList();
                if (children.Count == 1) {
                    var usage = GetSmartUsage(children.First(), source, childOptional, childOptional);
                    if (usage != null) {
                        return self + ArgumentSeparator + usage;
                    }
                } else if (children.Count > 1) {
                    HashSet<string> childUsage = new ();
                    foreach (var child in children) {
                        var usage = GetSmartUsage(child, source, childOptional, true);
                        if (usage != null) {
                            childUsage.Add(usage);
                        }
                    }
                    if (childUsage.Count == 1) {
                        var usage = childUsage.First();
                        return self + ArgumentSeparator + (childOptional ? UsageOptionalOpen + usage + UsageOptionalClose : usage);
                    } else if (childUsage.Count > 1) {
                        var builder = new StringBuilder(open);
                        var count = 0;
                        foreach (var child in children) {
                            if (count > 0) {
                                builder.Append(UsageOr);
                            }
                            builder.Append(child.GetUsageText());
                            count++;
                        }
                        if (count > 0) {
                            builder.Append(close);
                            return self + ArgumentSeparator + builder;
                        }
                    }
                }
            }
        }

        return self;
    }

    /**
     * Gets suggestions for a parsed input string on what comes next.
     *
     * <p>As it is ultimately up to custom argument types to provide suggestions, it may be an asynchronous operation,
     * for example getting in-game data or player names etc. As such, this method returns a future and no guarantees
     * are made to when or how the future completes.</p>
     *
     * <p>The suggestions provided will be in the context of the end of the parsed input string, but may suggest
     * new or replacement strings for earlier in the input string. For example, if the end of the string was
     * {@code foobar} but an argument preferred it to be {@code minecraft:foobar}, it will suggest a replacement for that
     * whole segment of the input.</p>
     *
     * @param parse the result of a {@link #parse(StringReader, Object)}
     * @return a future that will eventually resolve into a {@link Suggestions} object
     */
    public Task<Suggestions> GetCompletionSuggestions(ParseResults<TS> parse) {
        return GetCompletionSuggestions(parse, parse.GetReader().GetTotalLength());
    }

    public Task<Suggestions> GetCompletionSuggestions(ParseResults<TS> parse, int cursor) {
        var context = parse.GetContext();

        var nodeBeforeCursor = context.FindSuggestionContext(cursor);
        var parent = nodeBeforeCursor.Parent;
        var start = Math.Min(nodeBeforeCursor.StartPos, cursor);

        var fullInput = parse.GetReader().GetString();
        var truncatedInput = fullInput.Substring(0, cursor);
        var truncatedInputLowerCase = truncatedInput.ToLowerInvariant();
        var futures = new Task<Suggestions>[parent.GetChildren().Count()];
        var i = 0;
        foreach (var node in parent.GetChildren()) {
            var future = Suggestions.Empty();
            try {
                future = node.ListSuggestions(context.Build(truncatedInput), new SuggestionsBuilder(truncatedInput, truncatedInputLowerCase, start));
            } catch (CommandSyntaxException ignored) {
            }
            futures[i++] = future;
        }

        var completed = false;
        Suggestions? val = null;
        Task<Suggestions> result = Task.Run(async () =>
        {
            while (!completed) await Task.Yield();
            return val;
        });
        
        Task.WhenAll(futures).ContinueWith(task => {
            List<Suggestions> suggestions = new ();
            foreach (var future in futures) {
                suggestions.Add(future.Result);
            }
            val = Suggestions.Merge(fullInput, suggestions);
            completed = true;
        });

        return result;
    }

    /**
     * Gets the root of this command tree.
     *
     * <p>This is often useful as a target of a {@link com.mojang.brigadier.builder.ArgumentBuilder#redirect(CommandNode)},
     * {@link #getAllUsage(CommandNode, Object, bool)} or {@link #getSmartUsage(CommandNode, Object)}.
     * You may also use it to clone the command tree via {@link #CommandDispatcher(RootCommandNode)}.</p>
     *
     * @return root of the command tree
     */
    public RootCommandNode<TS> GetRoot() {
        return _root;
    }

    /**
     * Finds a valid path to a given node on the command tree.
     *
     * <p>There may theoretically be multiple paths to a node on the tree, especially with the use of forking or redirecting.
     * As such, this method makes no guarantees about which path it finds. It will not look at forks or redirects,
     * and find the first instance of the target node on the tree.</p>
     *
     * <p>The only guarantee made is that for the same command tree and the same version of this library, the result of
     * this method will <b>always</b> be a valid input for {@link #findNode(Collection)}, which should return the same node
     * as provided to this method.</p>
     *
     * @param target the target node you are finding a path for
     * @return a path to the resulting node, or an empty list if it was not found
     */
    public IEnumerable<string> GetPath(CommandNode<TS> target) {
        List<List<CommandNode<TS>>> nodes = new ();
        AddPaths(_root, nodes, new ());

        foreach (var list in nodes) {
            if (list[^1] == target) {
                List<string> result = new ();
                foreach (var node in list) {
                    if (node != _root) {
                        result.Add(node.GetName());
                    }
                }
                return result;
            }
        }

        return Array.Empty<string>();
    }

    /**
     * Finds a node by its path
     *
     * <p>Paths may be generated with {@link #getPath(CommandNode)}, and are guaranteed (for the same tree, and the
     * same version of this library) to always produce the same valid node by this method.</p>
     *
     * <p>If a node could not be found at the specified path, then {@code null} will be returned.</p>
     *
     * @param path a generated path to a node
     * @return the node at the given path, or null if not found
     */
    public CommandNode<TS>? FindNode(Collection<string> path) {
        CommandNode<TS>? node = _root;
        foreach (var name in path) {
            node = node.GetChild(name);
            if (node == null) {
                return null;
            }
        }
        return node;
    }

    /**
     * Scans the command tree for potential ambiguous commands.
     *
     * <p>This is a shortcut for {@link CommandNode#findAmbiguities(AmbiguityConsumer)} on {@link #getRoot()}.</p>
     *
     * <p>Ambiguities are detected by testing every {@link CommandNode#getExamples()} on one node verses every sibling
     * node. This is not fool proof, and relies a lot on the providers of the used argument types to give good examples.</p>
     *
     * @param consumer a callback to be notified of potential ambiguities
     */
    public void FindAmbiguities(AmbiguityConsumer<TS> consumer) {
        _root.FindAmbiguities(consumer);
    }

    private void AddPaths(CommandNode<TS> node, List<List<CommandNode<TS>>> result, List<CommandNode<TS>> parents) {
        List<CommandNode<TS>> current = new (parents);
        current.Add(node);
        result.Add(current);

        foreach (var child in node.GetChildren()) {
            AddPaths(child, result, current);
        }
    }
}
