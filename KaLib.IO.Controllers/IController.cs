using KaLib.Utils;

namespace KaLib.IO.Controllers;

public interface IController : IDisposable
{
    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    public event Action Disconnected;

    public bool Disposed { get; }
    public IOptional<IControllerSnapshot> LastSnapshot { get; }
    
    public void Initialize();
    public IControllerSnapshot PollInput();
    public void UpdateStates();
}

public interface IController<out T> : IController where T : IControllerSnapshot
{
    public event Action<T, T> SnapshotUpdated;
    public new IOptional<T> LastSnapshot { get; }
    public new T PollInput();
    IControllerSnapshot IController.PollInput() => PollInput();
    IOptional<IControllerSnapshot> IController.LastSnapshot => LastSnapshot.Select(t => (IControllerSnapshot)t);
}