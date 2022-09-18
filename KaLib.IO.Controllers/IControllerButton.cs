namespace KaLib.IO.Controllers;

public interface IControllerButton
{
    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    
    public bool Pressed { get; }
}