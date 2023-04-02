namespace KaLib.IO.Controllers.DualSense;

[Flags]
public enum DualSenseControlFlags : byte
{
    MicMuteLed = 1 << 0,
    PowerSave = 1 << 1,
    LightBar = 1 << 2,
    ReleaseLeds = 1 << 3,
    PlayerIndicator = 1 << 4
}