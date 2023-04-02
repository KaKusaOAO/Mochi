using System.Runtime.InteropServices;

namespace KaLib.IO.Controllers.XInput;

[StructLayout(LayoutKind.Sequential)]
public struct XInputGamepad
{
    public short Buttons;
    public byte LeftTrigger;
    public byte RightTrigger;
    public short LeftStickX;
    public short LeftStickY;
    public short RightStickX;
    public short RightStickY;
}