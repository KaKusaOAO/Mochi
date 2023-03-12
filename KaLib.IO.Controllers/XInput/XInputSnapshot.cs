namespace KaLib.IO.Controllers.XInput;

public struct XInputSnapshot : IBasicControllerSnapshot
{
    public IControllerStickButton.StickButtonState LeftStick { get; init; }
    public IControllerStickButton.StickButtonState RightStick { get; init; }
    public float LeftTrigger { get; init; }
    public float RightTrigger { get; init; }
    public bool ButtonA { get; init; }
    public bool ButtonB { get; init; }
    public bool ButtonX { get; init; }
    public bool ButtonY { get; init; }
    public bool LeftShoulder { get; init; }
    public bool RightShoulder { get; init; }
    public IControllerDPad.DPadState DPad { get; init; }
    public bool Share { get; init; }
    public bool Options { get; init; }
    public bool View { get; init; }
    public bool Home { get; init; }
}