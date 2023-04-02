namespace Mochi.Brigadier;

public class LiteralMessage : IBrigadierMessage
{
    private readonly string _str;

    public LiteralMessage(string str)
    {
        _str = str;
    }

    public string GetString()
    {
        return _str;
    }

    public override string ToString()
    {
        return _str;
    }
}