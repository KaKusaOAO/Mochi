namespace KaLib.IO.Controllers.DualSense;

public enum DualSenseBatteryStatus
{
    Discharging,
    Charging,
    Full,
    
    VoltOrTempOutOfRange = 0xa,
    TempError,
    
    ChargingError = 0xf
}