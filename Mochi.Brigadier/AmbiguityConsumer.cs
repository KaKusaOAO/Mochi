using System.Collections.Generic;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier;

public delegate void AmbiguityConsumer<T>(
    CommandNode<T> parent,
    CommandNode<T> child,
    CommandNode<T> sibling,
    IEnumerable<string> inputs);