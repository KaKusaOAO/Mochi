using System.Runtime.InteropServices;

namespace KaLib.IO.Controllers.XInput;

[StructLayout(LayoutKind.Sequential)]
public struct XInputVibration
{
    public short LeftRumble;
    public short RightRumble;
}