using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Context;

public class SuggestionContext<TS>
{
    public CommandNode<TS> Parent { get; }
    public int StartPos { get; }

    public SuggestionContext(CommandNode<TS> parent, int startPos)
    {
        Parent = parent;
        StartPos = startPos;
    }
}