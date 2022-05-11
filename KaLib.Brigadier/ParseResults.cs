using System.Collections.Generic;
using KaLib.Brigadier.Context;
using KaLib.Brigadier.Exceptions;
using KaLib.Brigadier.Tree;

namespace KaLib.Brigadier
{
    public class ParseResults<TS> {
        private readonly CommandContextBuilder<TS> _context;
        private readonly Dictionary<CommandNode<TS>, CommandSyntaxException> _exceptions;
        private readonly IMmutableStringReader _reader;

        public ParseResults(CommandContextBuilder<TS> context, IMmutableStringReader reader, Dictionary<CommandNode<TS>, CommandSyntaxException> exceptions) {
            this._context = context;
            this._reader = reader;
            this._exceptions = exceptions;
        }

        public ParseResults(CommandContextBuilder<TS> context) : this(context, new StringReader(""), new Dictionary<CommandNode<TS>, CommandSyntaxException>()) {
        
        }

        public CommandContextBuilder<TS> GetContext() {
            return _context;
        }

        public IMmutableStringReader GetReader() {
            return _reader;
        }

        public Dictionary<CommandNode<TS>, CommandSyntaxException> GetExceptions() {
            return _exceptions;
        }
    }
}