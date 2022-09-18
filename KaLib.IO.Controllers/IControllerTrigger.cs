namespace KaLib.IO.Controllers;

public interface IControllerTrigger : IControllerButton
{
    public float Value { get; }

    bool IControllerButton.Pressed => Value > 0;
}