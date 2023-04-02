namespace KaLib.IO.Controllers;

public interface IBasicControllerSnapshot : IControllerSnapshot
{
    IControllerStickButton.StickButtonState LeftStick { get; }
    IControllerStickButton.StickButtonState RightStick { get; }
    float LeftTrigger { get; }
    float RightTrigger { get; }
    bool ButtonA { get; }
    bool ButtonB { get; }
    bool ButtonX { get; }
    bool ButtonY { get; }
    bool LeftShoulder { get; }
    bool RightShoulder { get; }
    IControllerDPad.DPadState DPad { get; }
    bool Share { get; }
    bool Options { get; }
    bool View { get; }
    bool Home { get; }
}

public static class BasicControllerSnapshotEx
{
    public static bool IsLeftTriggerPressed(this IBasicControllerSnapshot snapshot,
        float threshold = IControllerTrigger.DefaultThreshold)
    {
        return snapshot.LeftTrigger >= threshold;
    }
    
    public static bool IsRightTriggerPressed(this IBasicControllerSnapshot snapshot,
        float threshold = IControllerTrigger.DefaultThreshold)
    {
        return snapshot.RightTrigger >= threshold;
    }
}

public interface IBatteryControllerSnapshot
{
    public float BatteryCapacity { get; }
    public BatteryStatus BatteryStatus { get; }
}