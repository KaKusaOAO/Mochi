namespace KaLib.IO.Controllers.DualSense;

public class DualSenseTrigger : IControllerTrigger
{
    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    private float _value;

    public float Value
    {
        get => _value;
        set => UpdatePressed(value);
    }

    private void UpdatePressed(float value)
    {
        if (_value == 0 != (value == 0))
        {
            if (value == 0)
            {
                ButtonReleased?.Invoke(this);
            }
            else
            {
                ButtonPressed?.Invoke(this);
            }
        }
        
        _value = value;
    }
}