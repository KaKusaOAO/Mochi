using System.Collections.Generic;
using System.Threading.Tasks;
using KaLib.Brigadier.Context;

namespace KaLib.Brigadier.Suggests
{
    public class SuggestionsBuilder {
        private readonly string _input;
        private readonly string _inputLowerCase;
        private readonly int _start;
        private readonly string _remaining;
        private readonly string _remainingLowerCase;
        private readonly List<Suggestion> _result = new List<Suggestion>();

        public SuggestionsBuilder(string input, string inputLowerCase, int start) {
            this._input = input;
            this._inputLowerCase = inputLowerCase;
            this._start = start;
#if NETCOREAPP
            this._remaining = input[start..];
            this._remainingLowerCase = inputLowerCase[start..];
#else
            this._remaining = input.Substring(start);
            this._remainingLowerCase = inputLowerCase.Substring(start);
#endif
        }

        public SuggestionsBuilder(string input, int start) : this(input, input.ToLowerInvariant(), start) {
        }

        public string GetInput() {
            return _input;
        }

        public int GetStart() {
            return _start;
        }

        public string GetRemaining() {
            return _remaining;
        }

        public string GetRemainingLowerCase() {
            return _remainingLowerCase;
        }

        public Suggestions Build() {
            return Suggestions.Create(_input, _result);
        }

        public async Task<Suggestions> BuildFuture()
        {
            await Task.Yield();
            return Build();
        }

        public SuggestionsBuilder Suggest(string text) {
            if (text.Equals(_remaining)) {
                return this;
            }
            _result.Add(new Suggestion(StringRange.Between(_start, _input.Length), text));
            return this;
        }

        public SuggestionsBuilder Suggest(string text, IMessage tooltip) {
            if (text.Equals(_remaining)) {
                return this;
            }
            _result.Add(new Suggestion(StringRange.Between(_start, _input.Length), text, tooltip));
            return this;
        }

        public SuggestionsBuilder Suggest(int value) {
            _result.Add(new IntegerSuggestion(StringRange.Between(_start, _input.Length), value));
            return this;
        }

        public SuggestionsBuilder Suggest(int value, IMessage tooltip) {
            _result.Add(new IntegerSuggestion(StringRange.Between(_start, _input.Length), value, tooltip));
            return this;
        }

        public SuggestionsBuilder Add(SuggestionsBuilder other) {
            _result.AddRange(other._result);
            return this;
        }

        public SuggestionsBuilder CreateOffset(int start) {
            return new SuggestionsBuilder(_input, _inputLowerCase, start);
        }

        public SuggestionsBuilder Restart() {
            return CreateOffset(_start);
        }
    }
}