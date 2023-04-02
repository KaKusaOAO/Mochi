namespace KaLib.IO.Controllers;

/// <summary>
/// A hybrid controller can be connected via USB or Bluetooth.
/// </summary>
public interface IHybridController : IController
{
    public ConnectionType ConnectionType { get; }
}