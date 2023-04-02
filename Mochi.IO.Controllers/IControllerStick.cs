using System.Numerics;

namespace KaLib.IO.Controllers;

public interface IControllerStick : IControllerStateComponent<Vector2>
{
    public Vector2 Vector { get; }

    Vector2 IControllerStateComponent<Vector2>.State => Vector;
}

public interface IControllerStickButton : IControllerStick, IControllerButton,
    IControllerStateComponent<IControllerStickButton.StickButtonState>
{
    public struct StickButtonState
    {
        public Vector2 Vector { get; init; }
        public bool Pressed { get; init; }

        public Vector2 GetFixedVector()
        {
            var len = Vector.Length();
            if (len <= 1) return Vector;
            return Vector * (1 / len);
        }
    }

    StickButtonState IControllerStateComponent<StickButtonState>.State => new()
    {
        Vector = Vector,
        Pressed = Pressed
    };

    Vector2 IControllerStateComponent<Vector2>.State =>
        ((IControllerStateComponent<StickButtonState>)this).State.Vector;
    
    bool IControllerStateComponent<bool>.State =>
        ((IControllerStateComponent<StickButtonState>)this).State.Pressed;
}