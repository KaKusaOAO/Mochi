namespace KaLib.IO.Controllers;

public class GeneralControllerButton : IControllerButton
{
    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    
    private bool _pressed;

    public GeneralControllerButton(string name = "Button")
    {
        Name = name;
    }

    public string Name { get; }

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

    public override string ToString()
    {
        var state = Pressed ? "Pressed" : "Released";
        return $"{Name}[{state}]";
    }
}