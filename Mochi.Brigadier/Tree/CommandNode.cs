using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mochi.Brigadier.Builder;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Suggests;

namespace Mochi.Brigadier.Tree
{
    public abstract class CommandNode<TS> : IComparable<CommandNode<TS>> {
        private readonly Dictionary<string, CommandNode<TS>> _children = new Dictionary<string, CommandNode<TS>>();
        private Dictionary<string, LiteralCommandNode<TS>> _literals = new Dictionary<string, LiteralCommandNode<TS>>();
        private Dictionary<string, ArgumentCommandNode<TS>> _arguments =
            new Dictionary<string, ArgumentCommandNode<TS>>();
        private Predicate<TS> _requirement;
        private CommandNode<TS> _redirect;
        private RedirectModifier<TS> _modifier;
        private bool _forks;
        private ICommand<TS> _command;

        protected CommandNode(ICommand<TS> command, Predicate<TS> requirement, CommandNode<TS> redirect, RedirectModifier<TS> modifier, bool forks) {
            this._command = command;
            this._requirement = requirement;
            this._redirect = redirect;
            this._modifier = modifier;
            this._forks = forks;
        }
    
        public ICommand<TS> Command => _command;

        public IEnumerable<CommandNode<TS>> GetChildren() {
            return _children.Values;
        }

        public CommandNode<TS> GetChild(string name)
        {
#if NET6_0_OR_GREATER
            return _children.GetValueOrDefault(name);
#else
            return _children.TryGetValue(name, out var node) ? node : null;
#endif
        }

        public CommandNode<TS> Redirect => _redirect;

        public RedirectModifier<TS> RedirectModifier => _modifier;

        public bool CanUse(TS source) {
            return _requirement(source);
        }

        public void AddChild(CommandNode<TS> node) {
            if (node is RootCommandNode<TS>) {
                throw new NotSupportedException("Cannot add a RootCommandNode as a child to any other CommandNode");
            }

#if NET6_0_OR_GREATER
            var child = _children.GetValueOrDefault(node.Name);
            if (child != null) 
#else
            if(_children.TryGetValue(node.Name, out var child))
#endif
            {
                // We've found something to merge onto
                if (node.Command != null) {
                    child._command = node.Command;
                }
                foreach (var grandchild in node.GetChildren()) {
                    child.AddChild(grandchild);
                }
            } else {
                _children.Add(node.Name, node);
                if (node is LiteralCommandNode<TS>) {
                    _literals.Add(node.Name, (LiteralCommandNode<TS>) node);
                } else if (node is ArgumentCommandNode<TS>) {
                    _arguments.Add(node.Name, (ArgumentCommandNode<TS>) node);
                }
            }
        }

        public void FindAmbiguities(AmbiguityConsumer<TS> consumer) {
            HashSet<string> matches = new HashSet<string>();

            foreach (var child in _children.Values) {
                foreach (var sibling in _children.Values) {
                    if (child == sibling) {
                        continue;
                    }

                    foreach (var input in child.GetExamples()) {
                        if (sibling.IsValidInput(input)) {
                            matches.Add(input);
                        }
                    }

                    if (matches.Count > 0) {
                        consumer(this, child, sibling, matches);
                        matches = new HashSet<string>();
                    }
                }

                child.FindAmbiguities(consumer);
            }
        }

        protected abstract bool IsValidInput(string input);

        public override bool Equals(object o) {
            if (this == o) return true;
            if (!(o is CommandNode<TS> that)) return false;

            if (!_children.Equals(that._children)) return false;
            if (!_command?.Equals(that._command) ?? that._command != null) return false;

            return true;
        }

        public override int GetHashCode() {
            return 31 * _children.GetHashCode() + (_command?.GetHashCode() ?? 0);
        }

        public Predicate<TS> Requirement => _requirement;

        public abstract string Name { get; }

        public abstract string GetUsageText();

        public abstract void Parse(StringReader reader, CommandContextBuilder<TS> contextBuilder);

        public abstract Task<Suggestions> ListSuggestions(CommandContext<TS> context, SuggestionsBuilder builder);

        public abstract ArgumentBuilder<TS> CreateBuilder();

        protected abstract string GetSortedKey();

        public IEnumerable<CommandNode<TS>> GetRelevantNodes(StringReader input)
        {
            if (_literals.Count > 0) {
                var cursor = input.GetCursor();
                while (input.CanRead() && input.Peek() != ' ') {
                    input.Skip();
                }
                var text = input.GetString().Substring(cursor, input.GetCursor() - cursor);
                input.SetCursor(cursor);
                
#if NET6_0_OR_GREATER
                var literal = _literals.GetValueOrDefault(text);
                if (literal != null)
#else
                if(_literals.TryGetValue(text, out var literal))
#endif
                {
                    return new [] { literal };
                }

                return _arguments.Values;
            }

            return _arguments.Values;
        }

        public int CompareTo(CommandNode<TS> o) {
            if (this is LiteralCommandNode<TS> == o is LiteralCommandNode<TS>) {
                return GetSortedKey().CompareTo(o.GetSortedKey());
            }

            return (o is LiteralCommandNode<TS>) ? 1 : -1;
        }

        public bool IsFork => _forks;

        public abstract IEnumerable<string> GetExamples();
    }
}
