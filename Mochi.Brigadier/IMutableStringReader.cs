namespace Mochi.Brigadier;

public interface IMutableStringReader
{
    string GetString();

    int RemainingLength { get; }

    int TotalLength { get; }

    int Cursor { get; }

    string GetRead();

    string GetRemaining();

    bool CanRead(int length);

    bool CanRead();

    char Peek();

    char Peek(int offset);
}