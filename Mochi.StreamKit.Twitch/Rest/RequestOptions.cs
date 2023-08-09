namespace Mochi.StreamKit.Twitch.Rest;

public class RequestOptions
{
    public int ExpectedPointsCost { get; set; } = 1;
    public bool HeaderOnly { get; set; }

    public IDictionary<string, IEnumerable<string>> Headers { get; } =
        new Dictionary<string, IEnumerable<string>>();
}