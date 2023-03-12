namespace KaLib.IO.Controllers.DualSense;

[Flags]
internal enum DualSenseButtonsB : byte
{
    LeftShoulder = 0x1,
    RightShoulder = 0x2,
    LeftTrigger = 0x4,
    RightTrigger = 0x8,
    Share = 0x10,
    Options = 0x20,
    LeftStick = 0x40,
    RightStick = 0x80
}