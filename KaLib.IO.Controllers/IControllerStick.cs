using System.Numerics;

namespace KaLib.IO.Controllers;

public interface IControllerStick
{
    public Vector2 Vector { get; }
}

public interface IControllerStickButton : IControllerStick, IControllerButton
{
    
}