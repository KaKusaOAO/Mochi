namespace KaLib.IO.Controllers;

public interface IControllerTrigger : IControllerButton, IControllerStateComponent<float>
{
    public const float DefaultThreshold = 0.3f;

    public float Value { get; }

    bool IControllerButton.Pressed => Value >= DefaultThreshold;
    float IControllerStateComponent<float>.State => Value;
}