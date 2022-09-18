using System.Runtime.InteropServices;

namespace KaLib.IO.Controllers.DualSense;

[StructLayout(LayoutKind.Explicit)]
internal unsafe struct DualSenseOutputState
{
    // -3
    
    [FieldOffset(0)] public byte RightRumble;
    [FieldOffset(1)] public byte LeftRumble;

    [FieldOffset(6)] public byte MicLed;
    [FieldOffset(7)] public byte MicFlag;
    [FieldOffset(8)] public AdaptiveTriggerState RightAdaptiveTriggerState;
    [FieldOffset(19)] public AdaptiveTriggerState LeftAdaptiveTriggerState;

    [FieldOffset(40)] public byte Brightness;
    [FieldOffset(41)] public PlayerLedMode PlayerLed;
    [FieldOffset(42)] public byte TouchpadColorR;
    [FieldOffset(43)] public byte TouchpadColorG;
    [FieldOffset(44)] public byte TouchpadColorB;
}

[StructLayout(LayoutKind.Explicit, Size = 11)]
internal unsafe struct AdaptiveTriggerState
{
    [FieldOffset(0)]
    public AdaptiveTriggerMode Mode;
    
    [FieldOffset(1)]
    public AdaptiveTriggerForces Forces;
}

[StructLayout(LayoutKind.Explicit, Size = 9)]
internal struct AdaptiveTriggerForces
{
    [FieldOffset(0)] public byte Force0;
    [FieldOffset(1)] public byte Force1;
    [FieldOffset(2)] public byte Force2;
    [FieldOffset(3)] public byte Force3;
    [FieldOffset(4)] public byte Force4;
    [FieldOffset(5)] public byte Force5;
    [FieldOffset(8)] public byte Force6;
}