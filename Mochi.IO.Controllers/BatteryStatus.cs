namespace KaLib.IO.Controllers;

public enum BatteryStatus
{
    Unknown,
    Discharging,
    Charging,
    Full,
    
    VoltOrTempOutOfRange,
    TempError
}