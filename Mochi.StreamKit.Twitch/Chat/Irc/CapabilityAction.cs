namespace Mochi.StreamKit.Twitch.Chat.Irc;

public enum CapabilityAction
{
    Request, Ack, NoAck
}

public static class CapabilityActions
{
    public static string ToIrcArgument(this CapabilityAction action)
    {
        return action switch
        {
            CapabilityAction.Request => "REQ",
            CapabilityAction.Ack => "* ACK",
            CapabilityAction.NoAck => "* NAK",
            _ => throw new ArgumentOutOfRangeException(nameof(action))
        };
    }

    public static CapabilityAction FromIrcArgument(string text)
    {
        return text switch
        {
            "REQ" => CapabilityAction.Request,
            "* ACK" => CapabilityAction.Ack,
            "* NAK" => CapabilityAction.NoAck,
            _ => throw new Exception($"Unknown cap action: {text}")
        };
    }
}