namespace KaLib.IO.Controllers;

public interface IController : IDisposable
{
    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    public event Action Disconnected;

    IControllerSnapshot Update();
    void UpdateStates();
}

public interface IController<out T> : IController where T : IControllerSnapshot
{
    public event Action<T, T> SnapshotUpdated;
    
    new T PollInput();
    IControllerSnapshot IController.Update() => PollInput();
}

public interface IControllerSnapshot
{
}

public interface IHybridController : IController
{
    public ConnectionType ConnectionType { get; }
}