using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Context;

public class SuggestionContext<TS> {
    public readonly CommandNode<TS> Parent;
    public readonly int StartPos;

    public SuggestionContext(CommandNode<TS> parent, int startPos) {
        this.Parent = parent;
        this.StartPos = startPos;
    }
}