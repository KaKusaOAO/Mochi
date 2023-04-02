using System.Numerics;

namespace KaLib.IO.Controllers.DualSense;

public struct DualSenseSnapshot : IBasicControllerSnapshot, IBatteryControllerSnapshot
{
    public IControllerStickButton.StickButtonState LeftStick { get; init; }
    public IControllerStickButton.StickButtonState RightStick { get; init; }
    public float LeftTrigger { get; init; }
    public float RightTrigger { get; init; }
    public bool ButtonCross { get; init; }
    public bool ButtonCircle { get; init; }
    public bool ButtonSquare { get; init; }
    public bool ButtonTriangle { get; init; }
    public bool LeftShoulder { get; init; }
    public bool RightShoulder { get; init; }
    public IControllerDPad.DPadState DPad { get; init; }
    public bool Share { get; init; }
    public bool Options { get; init; }
    public DualSenseTouchPad.TouchPadState TouchPad { get; init; }
    public bool PlayStationLogo { get; init; }
    public bool Mic { get; init; }
    public Vector3 Accelerometer { get; init; }
    public Vector3 Gyroscope { get; init; }
    
    public float BatteryCapacity { get; init; }
    public DualSenseBatteryStatus BatteryStatus { get; init; }

    bool IBasicControllerSnapshot.ButtonA => ButtonCross;
    bool IBasicControllerSnapshot.ButtonB => ButtonCircle;
    bool IBasicControllerSnapshot.ButtonX => ButtonSquare;
    bool IBasicControllerSnapshot.ButtonY => ButtonTriangle;
    bool IBasicControllerSnapshot.View => TouchPad.Pressed;
    bool IBasicControllerSnapshot.Home => PlayStationLogo;

    BatteryStatus IBatteryControllerSnapshot.BatteryStatus
    {
        get
        {
            return BatteryStatus switch
            {
                DualSenseBatteryStatus.Charging => Controllers.BatteryStatus.Charging,
                DualSenseBatteryStatus.Discharging => Controllers.BatteryStatus.Discharging,
                DualSenseBatteryStatus.Full => Controllers.BatteryStatus.Full,
                DualSenseBatteryStatus.VoltOrTempOutOfRange => Controllers.BatteryStatus.VoltOrTempOutOfRange,
                DualSenseBatteryStatus.TempError => Controllers.BatteryStatus.TempError,
                DualSenseBatteryStatus.ChargingError => Controllers.BatteryStatus.Unknown,
                _ => Controllers.BatteryStatus.Unknown
            };
        }
    }
}