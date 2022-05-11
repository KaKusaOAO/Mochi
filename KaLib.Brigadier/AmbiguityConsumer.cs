using System.Collections.Generic;
using KaLib.Brigadier.Tree;

namespace KaLib.Brigadier
{
    public delegate void AmbiguityConsumer<TS>(CommandNode<TS> parent, CommandNode<TS> child, CommandNode<TS> sibling, IEnumerable<string> inputs);
}