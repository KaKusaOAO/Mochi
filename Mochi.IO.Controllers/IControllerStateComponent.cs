namespace KaLib.IO.Controllers;

public interface IControllerStateComponent
{
}

public interface IControllerStateComponent<out T> : IControllerStateComponent
{
    T State { get; }
}

public static class ControllerStateComponentEx
{
    public static T GetState<T>(this IControllerStateComponent<T> component) => component.State;
}