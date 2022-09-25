namespace KaLib.IO.Controllers;

public interface IControllerButton : IControllerStateComponent<bool>
{
    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    
    public bool Pressed { get; }

    bool IControllerStateComponent<bool>.State => Pressed;
}