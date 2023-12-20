namespace Mochi.Metal;

public enum MTLDeviceLocation : ulong
{
    BuiltIn,
    Slot,
    External,
    Unspecified = ulong.MaxValue, 
}