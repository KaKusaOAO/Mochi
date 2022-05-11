using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KaLib.Brigadier.Context;

namespace KaLib.Brigadier.Suggests
{
    public class Suggestions {
        private static readonly Suggestions InternalEmpty = new Suggestions(StringRange.At(0), new List<Suggestion>());

        private readonly StringRange _range;
        private readonly List<Suggestion> _suggestions;

        public Suggestions(StringRange range, List<Suggestion> suggestions) {
            this._range = range;
            this._suggestions = suggestions;
        }

        public StringRange GetRange() {
            return _range;
        }

        public List<Suggestion> GetList() {
            return _suggestions;
        }

        public bool IsEmpty() {
            return _suggestions.Count == 0;
        }

        public override bool Equals(object o) {
            if (this == o) {
                return true;
            }
            if (!(o is Suggestions that)) {
                return false;
            }

            return _range == that._range &&
                   _suggestions == that._suggestions;
        }

        public override int GetHashCode() {
#if NET6_0_OR_GREATER
            return HashCode.Combine(_range, _suggestions);
#else
            return _range.GetHashCode() * 31 + _suggestions.GetHashCode();
#endif
        }

        public override string ToString() {
            return "Suggestions{" +
                   "range=" + _range +
                   ", suggestions=" + _suggestions +
                   '}';
        }

        public static async Task<Suggestions> Empty()
        {
            await Task.CompletedTask;
            return InternalEmpty;
        }

        public static Suggestions Merge(string command, ICollection<Suggestions> input) {
            if (input.Count == 0) {
                return InternalEmpty;
            } else if (input.Count == 1) {
                return input.First();
            }

            HashSet<Suggestion> texts = new HashSet<Suggestion>();
            foreach (var suggestions in input) {
                foreach (var item in suggestions.GetList())
                {
                    texts.Add(item);
                }
            }
            return Create(command, texts);
        }

        public static Suggestions Create(string command, ICollection<Suggestion> suggestions) {
            if (suggestions.Count == 0) {
                return InternalEmpty;
            }
            var start = int.MaxValue;
            var end = int.MinValue;
            foreach (var suggestion in suggestions) {
                start = Math.Min(suggestion.GetRange().GetStart(), start);
                end = Math.Max(suggestion.GetRange().GetEnd(), end);
            }
            var range = new StringRange(start, end);
            HashSet<Suggestion> texts = new HashSet<Suggestion>();
            foreach (var suggestion in suggestions) {
                texts.Add(suggestion.Expand(command, range));
            }
            List<Suggestion> sorted = new List<Suggestion>(texts);
            sorted.Sort((a, b) => a.CompareToIgnoreCase(b));
            return new Suggestions(range, sorted);
        }
    }
}
