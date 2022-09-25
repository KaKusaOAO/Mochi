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
    [FieldOffset(10)] public byte ButtonsD; // According to Linux driver

    // -> 4 bytes padding
    
    [FieldOffset(15)] public fixed short Gyroscope[3];
    [FieldOffset(21)] public fixed short Accelerometer[3];
    [FieldOffset(27)] public uint SensorTimestamp;
    
    // -> 1 byte padding
    
    [FieldOffset(32)] public uint Touch1;
    [FieldOffset(36)] public uint Touch2;

    [FieldOffset(42)] public byte LeftTriggerFeedback;
    [FieldOffset(41)] public byte RightTriggerFeedback;

    [FieldOffset(52)] public byte Status;
}