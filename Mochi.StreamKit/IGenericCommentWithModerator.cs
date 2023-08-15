namespace Mochi.StreamKit;

public interface IGenericCommentWithModerator : IGenericComment
{
    public bool IsModerator { get; }
}