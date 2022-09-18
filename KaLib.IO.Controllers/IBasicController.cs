namespace KaLib.IO.Controllers;

public interface IBasicController : IController
{
    IControllerStickButton LeftStick { get; }
    IControllerStickButton RightStick { get; }
    IControllerTrigger LeftTrigger { get; }
    IControllerTrigger RightTrigger { get; }
    IControllerButton ButtonA { get; }
    IControllerButton ButtonB { get; }
    IControllerButton ButtonX { get; }
    IControllerButton ButtonY { get; }
    IControllerButton LeftShoulder { get; }
    IControllerButton RightShoulder { get; }
    IControllerDPad DPad { get; }
    IControllerButton Share { get; }
    IControllerButton Options { get; }
    IControllerButton Function { get; }
    IControllerButton Home { get; }

    IEnumerable<IControllerButton> IController.Buttons
    {
        get
        {
            return new[]
            {
                LeftStick, RightStick, 
                LeftTrigger, RightTrigger,
                LeftShoulder, RightShoulder,
                ButtonA, ButtonB, ButtonX, ButtonY,
                DPad.Up, DPad.Down, DPad.Left, DPad.Right,
                Share, Options, Function, Home
            };
        }
    }
}