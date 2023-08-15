using System;
using System.Collections.Generic;

namespace Mochi.Texts;

public interface IComponentResolver
{
    public ICollection<IResolvedComponentEntry> GetResolvedEntries(string content);
}

public interface IResolvedComponentEntry
{
    public Range Range { get; }
    public IComponent? Resolve(IStyle style);
}