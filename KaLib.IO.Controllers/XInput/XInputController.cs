using System.Numerics;
using KaLib.IO.Controllers.XInput.Natives;
using KaLib.Utils;

namespace KaLib.IO.Controllers.XInput;

public class XInputController : IController<XInputSnapshot>, ITwoSideRumbleController<XInputRumble>
{
    public PlayerIndex Index { get; }
    public event Action<IControllerButton>? ButtonPressed;
    public event Action<IControllerButton>? ButtonReleased;
    public event Action<XInputSnapshot, XInputSnapshot>? SnapshotUpdated;
    public event Action? Disconnected;

    public XInputRumble LeftRumble { get; } = new();
    public XInputRumble RightRumble { get; } = new();
    public bool Disposed { get; private set; }

    public XInputController(PlayerIndex index)
    {
        Index = index;
    }

    public override int GetHashCode()
    {
        return Index.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not XInputController c) return false;
        return Index == c.Index;
    }

    public static IEnumerable<XInputController> FindAll()
    {
        return Enum.GetValues<PlayerIndex>().Select(index =>
            {
                try
                {
                    Natives.XInput.GetState(index);
                    return new XInputController(index);
                }
                catch (XInputException)
                {
                    return null;
                }
            })
            .Where(n => n != null)
            .Select(n => n!);
    }

    public IOptional<XInputSnapshot> LastSnapshot { get; private set; } = Optional.Empty<XInputSnapshot>();

    public void Initialize()
    {
        
    }

    public XInputSnapshot PollInput()
    {
        try
        {
            var state = Natives.XInput.GetState(Index);
            var pad = state.Gamepad;
            var snapshot = new XInputSnapshot
            {
                LeftStick = new IControllerStickButton.StickButtonState
                {
                    Vector = new Vector2(pad.LeftStickX * 1f / short.MaxValue, pad.LeftStickY * 1f / short.MaxValue)
                },
                RightStick = new IControllerStickButton.StickButtonState
                {
                    Vector = new Vector2(pad.RightStickX * 1f / short.MaxValue, pad.RightStickY * 1f / short.MaxValue)
                },
                LeftTrigger = pad.LeftTrigger * 1f / 255,
                RightTrigger = pad.RightTrigger * 1f / 255
            };
            LastSnapshot = Optional.Of(snapshot);
            return snapshot;
        }
        catch (XInputException)
        {
            Disconnected?.Invoke();
            return default;
        }
    }

    public void UpdateStates()
    {
        try
        {
            Natives.XInput.SetState(Index, LeftRumble.Amplitude, RightRumble.Amplitude);
        }
        catch (XInputException)
        {
            Disconnected?.Invoke();
        }
    }

    public void Dispose()
    {
        Disposed = true;
        
        try
        {
            Natives.XInput.SetState(Index, 0, 0);
        }
        catch (XInputException)
        {
            // Ignored
        }
    }
}