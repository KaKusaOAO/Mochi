namespace KaLib.IO.Controllers.DualSense;

[Flags]
internal enum DualSenseButtonsC : byte
{
    Home = 0x1,
    PadButton = 0x2,
    MicButton = 0x4
}