using System.Runtime.InteropServices;

namespace KaLib.IO.Controllers.DualSense;

[StructLayout(LayoutKind.Explicit)]
internal unsafe struct DualSenseInputState
{
    [FieldOffset(0)] public byte LeftStickX;
    [FieldOffset(1)] public byte LeftStickY;

    [FieldOffset(2)] public byte RightStickX;
    [FieldOffset(3)] public byte RightStickY;

    [FieldOffset(4)] public byte LeftTrigger;
    [FieldOffset(5)] public byte RightTrigger;

    [FieldOffset(7)] public byte ButtonsA;
    [FieldOffset(8)] public DualSenseButtonsB ButtonsB;
    [FieldOffset(9)] public DualSenseButtonsC ButtonsC;

    [FieldOffset(0x0f)] public fixed short Accelerometer[3];
    [FieldOffset(0x15)] public fixed short Gyroscope[3];

    [FieldOffset(0x20)] public uint Touch1;
    [FieldOffset(0x24)] public uint Touch2;

    [FieldOffset(0x35)] public byte Status1;
    [FieldOffset(0x36)] public byte Status2;

    [FieldOffset(0x2a)] public byte LeftTriggerFeedback;
    [FieldOffset(0x29)] public byte RightTriggerFeedback;
}