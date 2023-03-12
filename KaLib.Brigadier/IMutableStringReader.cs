namespace KaLib.Brigadier
{
    public interface IMutableStringReader
    {
        string GetString();

        int GetRemainingLength();

        int GetTotalLength();

        int GetCursor();

        string GetRead();

        string GetRemaining();

        bool CanRead(int length);

        bool CanRead();

        char Peek();

        char Peek(int offset);
    }
}