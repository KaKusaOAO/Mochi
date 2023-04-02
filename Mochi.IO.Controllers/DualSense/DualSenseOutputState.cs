using System.Runtime.InteropServices;

namespace KaLib.IO.Controllers.DualSense;

[StructLayout(LayoutKind.Explicit)]
internal unsafe struct DualSenseOutputState
{
    public const int Size = 47;
    
    // -3
    [FieldOffset(0)] public byte Flag1;
    [FieldOffset(1)] public DualSenseControlFlags ControlFlags;
    
    [FieldOffset(2)] public byte RightRumble;
    [FieldOffset(3)] public byte LeftRumble;
    
    [FieldOffset(4)] public float AudioF;
    [FieldOffset(4)] public byte Audio1;
    [FieldOffset(5)] public byte Audio2;
    [FieldOffset(6)] public byte Audio3;
    [FieldOffset(7)] public byte Audio4;
    
    [FieldOffset(8)] public byte MicLed;
    [FieldOffset(9)] public byte MicFlag;
    [FieldOffset(10)] public AdaptiveTriggerState RightAdaptiveTriggerState;
    [FieldOffset(21)] public AdaptiveTriggerState LeftAdaptiveTriggerState;
    
    [FieldOffset(41)] public byte LightbarSetup;
    [FieldOffset(42)] public byte Brightness;
    [FieldOffset(43)] public PlayerLedMode PlayerLed;
    [FieldOffset(44)] public byte LightBarColorR;
    [FieldOffset(45)] public byte LightBarColorG;
    [FieldOffset(46)] public byte LightBarColorB;
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