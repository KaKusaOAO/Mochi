using System.Runtime.InteropServices;

namespace KaLib.IO.Controllers.XInput;

[StructLayout(LayoutKind.Sequential)]
public struct XInputBatteryInfo
{
    public BatteryType Type;
    public BatteryLevel Level;
}

public enum BatteryType : byte
{
    Disconnected,
    Wired,
    Alkaline,
    NiMH,   // nickel metal hydride
    Unknown
}

public enum BatteryLevel : byte
{
    Empty,
    Low,
    Medium,
    Full
}