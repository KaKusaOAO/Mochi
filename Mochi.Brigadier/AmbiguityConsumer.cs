using System.Collections.Generic;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier;

public delegate void AmbiguityConsumer<TS>(CommandNode<TS> parent, CommandNode<TS> child, CommandNode<TS> sibling, IEnumerable<string> inputs);