namespace KaLib.IO.Controllers;



public interface ITwoSideRumbleController
{
    IControllerRumble LeftRumble { get; }
    IControllerRumble RightRumble { get; }
}

public interface ITwoSideRumbleController<out TLeft, out TRight> : ITwoSideRumbleController
    where TLeft : IControllerRumble where TRight : IControllerRumble
{
    new TLeft LeftRumble { get; }
    new TRight RightRumble { get; }

    IControllerRumble ITwoSideRumbleController.LeftRumble => LeftRumble;
    IControllerRumble ITwoSideRumbleController.RightRumble => RightRumble;
}

public interface ITwoSideRumbleController<out T> : ITwoSideRumbleController<T, T> where T : IControllerRumble
{
}