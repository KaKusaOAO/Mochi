namespace KaLib.IO.Controllers.DualSense;

[Flags]
internal enum DualSenseButtonsA : byte
{
    Square = 0x10,
    Cross = 0x20,
    Circle = 0x40,
    Triangle = 0x80
}