using Mochi.Structs;

namespace KaLib.IO.Controllers.DualSense;

public class DualSenseTouchPad : IControllerButton, IControllerStateComponent<DualSenseTouchPad.TouchPadState>
{
    private bool _pressed;
    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    public Color LightBar { get; set; } = Color.White;
    public PlayerLedMode PlayerLed { get; set; } = PlayerLedMode.One;
    public TouchState[] TouchStates { get; } = new TouchState[2];

    public bool Pressed
    {
        get => _pressed;
        set => UpdatePressed(value);
    }
    
    private void UpdatePressed(bool value)
    {
        if (_pressed == value) return;
        
        if (value)
        {
            ButtonPressed?.Invoke(this);
        }
        else
        {
            ButtonReleased?.Invoke(this);
        }

        _pressed = value;
    }

    public struct TouchPadState
    {
        public bool Pressed { get; init; }
        public TouchState[] TouchStates { get; init; }
    }

    public TouchPadState State => new()
    {
        TouchStates = new [] { TouchStates[0], TouchStates[1] },
        Pressed = Pressed
    };
}