namespace KaLib.IO.Controllers.DualSense;

[Flags]
public enum AdaptiveTriggerMode : byte
{
    None,
    Continuous,
    Section,
    
    A = 0x20,
    B = 0x04,
    
    Calibration = 0xfc
}