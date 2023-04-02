using System.Numerics;

namespace KaLib.IO.Controllers;

public interface IControllerDPad : IControllerStateComponent<IControllerDPad.DPadState>
{
    IControllerButton Up { get; }
    IControllerButton Down { get; }
    IControllerButton Left { get; }
    IControllerButton Right { get; }

    /// <summary>
    /// Returns a direction represents the buttons the players are pressing.
    /// </summary>
    /// <remarks>
    /// The X component will be left to right, and the Y component will be down to up.
    /// </remarks>
    public Vector2 Direction
    {
        get
        {
            var x = Left.Pressed ? -1 : Right.Pressed ? 1 : 0;
            var y = Up.Pressed ? 1 : Down.Pressed ? -1 : 0;
            var v = new Vector2(x, y);
            return v / v.Length();
        }
    }

    DPadState IControllerStateComponent<DPadState>.State => new()
    {
        Up = Up.Pressed,
        Down = Down.Pressed,
        Left = Left.Pressed,
        Right = Right.Pressed
    };

    public struct DPadState
    {
        public bool Up { get; init; }
        public bool Down { get; init; }
        public bool Left { get; init; }
        public bool Right { get; init; }
    }
}