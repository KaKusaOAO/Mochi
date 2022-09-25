namespace KaLib.IO.Hid.Platform.Windows.Natives;

public enum ConfigRet : uint
{
    Success,
    Failure = 0x13,
    BufferSmall = 0x1a
}