namespace KaLib.IO.Controllers;

public class GenericDPad : IControllerDPad
{
    public GeneralControllerButton Up { get; } = new();
    public GeneralControllerButton Down { get; } = new();
    public GeneralControllerButton Left { get; } = new();
    public GeneralControllerButton Right { get; } = new();

    IControllerButton IControllerDPad.Up => Up;
    IControllerButton IControllerDPad.Down => Down;
    IControllerButton IControllerDPad.Left => Left;
    IControllerButton IControllerDPad.Right => Right;
}