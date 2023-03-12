using System.Numerics;

namespace KaLib.IO.Controllers;

public class GenericStick : IControllerStickButton
{
    private bool _pressed;
    public event Action<IControllerButton> ButtonPressed;
    public event Action<IControllerButton> ButtonReleased;
    
    public GenericStick(string name = "Button")
    {
        Name = name;
    }

    public string Name { get; set; }

    public Vector2 Vector { get; set; }

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
        return $"{Name}[{Vector},{state}]";
    }
}