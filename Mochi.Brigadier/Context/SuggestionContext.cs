using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Context;

public class SuggestionContext<T>
{
    public CommandNode<T> Parent { get; }
    public int StartPos { get; }

    public SuggestionContext(CommandNode<T> parent, int startPos)
    {
        Parent = parent;
        StartPos = startPos;
    }
}