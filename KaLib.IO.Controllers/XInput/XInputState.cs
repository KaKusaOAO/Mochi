using System.Runtime.InteropServices;

namespace KaLib.IO.Controllers.XInput;

[StructLayout(LayoutKind.Sequential)]
public struct XInputState
{
    public int PacketNumber;
    public XInputGamepad Gamepad;
}