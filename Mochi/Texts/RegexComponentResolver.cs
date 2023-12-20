using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mochi.Texts;

public class RegexComponentResolver : IComponentResolver
{
    private readonly Regex _regex;
    private readonly Func<Match, IStyle, IComponent?> _factory;

    public RegexComponentResolver(Regex regex, Func<Match, IStyle, IComponent?> factory)
    {
        _regex = regex;
        _factory = factory;
    }
    
    public ICollection<IResolvedComponentEntry> GetResolvedEntries(string content) => _regex.Matches(content)
        .Select(match => new RegexResolvedComponentEntry(match, _factory))
        .OfType<IResolvedComponentEntry>().ToList();
}

public class RegexResolvedComponentEntry : IResolvedComponentEntry
{
    private readonly Match _match;
    private readonly Func<Match, IStyle, IComponent?> _factory;
    public Range Range => new(_match.Index, _match.Index + _match.Length);

    public RegexResolvedComponentEntry(Match match, Func<Match, IStyle, IComponent?> factory)
    {
        _match = match;
        _factory = factory;
    }

    public IComponent? Resolve(IStyle style) => _factory(_match, style);
}