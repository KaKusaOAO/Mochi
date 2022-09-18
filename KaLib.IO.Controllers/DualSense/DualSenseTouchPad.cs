

using KaLib.Structs;

namespace KaLib.IO.Controllers.DualSense;

public class DualSenseTouchPad : IControllerButton
{
    private bool _pressed;
    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    public Color LedColor { get; set; } = Color.White;
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
}