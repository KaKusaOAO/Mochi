namespace Mochi.StreamKit;

public interface ICommentWithModerator : IComment
{
    public bool IsModerator { get; }
}