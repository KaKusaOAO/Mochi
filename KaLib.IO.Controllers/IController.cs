namespace KaLib.IO.Controllers;

public interface IController : IDisposable
{
    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    public event Action Disconnected;
    
    IEnumerable<IControllerButton> Buttons { get; }

    void PollEvents();
    void SendStates();
    void Update();
}